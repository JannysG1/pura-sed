using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuraSed.WebApi.Constants;
using PuraSed.WebApi.Data;
using PuraSed.WebApi.DTOs;
using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PedidosController : ControllerBase
{
    private readonly PuraSedDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PedidosController(PuraSedDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// HU-13: Registrar pedido por mesa.
    /// Permitido a: Mesero (atención presencial en mesas), Cliente (desde la tablet de su mesa) y Admin.
    /// HU-14: Distribuye automáticamente cada ítem a "Cocina" o "Bar" según su categoría.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.TomaPedidos)]
    public async Task<ActionResult> CrearPedido([FromBody] CrearPedidoDTO dto)
    {
        if (dto.Items == null || dto.Items.Count == 0)
        {
            return BadRequest(new { mensaje = "El pedido debe contener al menos un producto." });
        }

        // Validar mesa
        var mesa = await _context.Mesas.FirstOrDefaultAsync(m => m.Numero == dto.NumeroMesa);
        if (mesa == null)
        {
            return NotFound(new { mensaje = $"La mesa {dto.NumeroMesa} no existe." });
        }

        var user = await _userManager.GetUserAsync(User);
        bool esMesero = User.IsInRole(Roles.Mesero);
        bool esCliente = User.IsInRole(Roles.Cliente);

        var pedido = new Pedido
        {
            NumeroMesa = dto.NumeroMesa,
            FechaHora = DateTime.UtcNow,
            Estado = "Pendiente",
            Observaciones = dto.Observaciones,
            MeseroId = esMesero ? user?.Id : mesa.MeseroAsignadoId,
            MeseroNombre = esMesero ? (user?.NombreCompleto ?? user?.UserName) : mesa.MeseroAsignadoNombre,
            ClienteUsuario = esCliente ? user?.UserName : null
        };

        decimal total = 0;

        foreach (var item in dto.Items)
        {
            var producto = await _context.Productos.FindAsync(item.ProductoId);
            if (producto == null)
            {
                return BadRequest(new { mensaje = $"El producto con ID {item.ProductoId} no existe." });
            }

            if (!producto.Activo)
            {
                return BadRequest(new { mensaje = $"El producto '{producto.Nombre}' está agotado y no puede ordenarse." });
            }

            // HU-14: Distribución automática según categoría
            string catLower = producto.Categoria.ToLowerInvariant();
            string area = (catLower.Contains("cerveza") || catLower.Contains("trago") || catLower.Contains("fermento") || catLower.Contains("chicha") || catLower.Contains("bebida") || catLower.Contains("bar"))
                ? "Bar"
                : "Cocina";

            decimal subtotal = producto.Precio * item.Cantidad;
            total += subtotal;

            pedido.Detalles.Add(new DetallePedido
            {
                ProductoId = producto.Id,
                ProductoNombre = producto.Nombre,
                Area = area,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.Precio,
                Subtotal = subtotal,
                EstadoItem = "Pendiente",
                Observaciones = item.Observaciones
            });
        }

        pedido.Total = total;

        // Actualizar estado de la mesa a Ocupada
        if (mesa.Estado == "Disponible")
        {
            mesa.Estado = "Ocupada";
            if (esMesero && user != null)
            {
                mesa.MeseroAsignadoId = user.Id;
                mesa.MeseroAsignadoNombre = user.NombreCompleto;
            }
        }

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObtenerPorId), new { id = pedido.Id }, pedido);
    }

    /// <summary>
    /// Consulta de pedidos según el rol del usuario (KDS Cocina, KDD Bar, Mesero o Gerencia).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = Roles.TodosPersonal)]
    public async Task<ActionResult> ObtenerPedidos([FromQuery] string? estado = null)
    {
        IQueryable<Pedido> query = _context.Pedidos
            .Include(p => p.Detalles)
            .OrderByDescending(p => p.FechaHora);

        if (!string.IsNullOrEmpty(estado))
        {
            query = query.Where(p => p.Estado == estado);
        }

        // Si es Cocina, filtramos para que solo vea los pedidos con ítems dirigidos a Cocina (HU-15)
        if (User.IsInRole(Roles.AdminCocina) && !User.IsInRole(Roles.Admin) && !User.IsInRole(Roles.SuperAdmin))
        {
            var pedidosCocina = await query
                .Where(p => p.Detalles.Any(d => d.Area == "Cocina"))
                .Select(p => new
                {
                    p.Id,
                    p.NumeroMesa,
                    p.FechaHora,
                    p.Estado,
                    p.MeseroNombre,
                    p.Observaciones,
                    ItemsCocina = p.Detalles.Where(d => d.Area == "Cocina").ToList()
                })
                .ToListAsync();

            return Ok(pedidosCocina);
        }

        // Si es Bar, filtramos para que solo vea los pedidos con ítems dirigidos a Bar (HU-15)
        if (User.IsInRole(Roles.AdminBar) && !User.IsInRole(Roles.Admin) && !User.IsInRole(Roles.SuperAdmin))
        {
            var pedidosBar = await query
                .Where(p => p.Detalles.Any(d => d.Area == "Bar"))
                .Select(p => new
                {
                    p.Id,
                    p.NumeroMesa,
                    p.FechaHora,
                    p.Estado,
                    p.MeseroNombre,
                    p.Observaciones,
                    ItemsBar = p.Detalles.Where(d => d.Area == "Bar").ToList()
                })
                .ToListAsync();

            return Ok(pedidosBar);
        }

        var pedidos = await query.ToListAsync();
        return Ok(pedidos);
    }

    /// <summary>
    /// HU-16: Seguimiento del pedido por ID.
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = Roles.TomaPedidos)]
    public async Task<ActionResult<Pedido>> ObtenerPorId(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Detalles)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
        {
            return NotFound(new { mensaje = $"Pedido #{id} no encontrado." });
        }

        return Ok(pedido);
    }

    /// <summary>
    /// HU-16: Consultar los pedidos de una mesa en tiempo real.
    /// </summary>
    [HttpGet("mesa/{numeroMesa}")]
    [Authorize(Roles = Roles.TomaPedidos)]
    public async Task<ActionResult> ObtenerPorMesa(int numeroMesa)
    {
        var pedidos = await _context.Pedidos
            .Include(p => p.Detalles)
            .Where(p => p.NumeroMesa == numeroMesa)
            .OrderByDescending(p => p.FechaHora)
            .ToListAsync();

        return Ok(pedidos);
    }

    /// <summary>
    /// HU-15: Cocina y Bar actualizan el estado individual de su ítem ("EnPreparacion", "Listo").
    /// </summary>
    [HttpPatch("{pedidoId}/items/{itemId}/estado")]
    [Authorize(Roles = Roles.CocinaYBar)]
    public async Task<ActionResult> ActualizarEstadoItem(int pedidoId, int itemId, [FromBody] ActualizarEstadoItemDTO dto)
    {
        var item = await _context.DetallesPedido
            .Include(d => d.Pedido)
            .FirstOrDefaultAsync(d => d.Id == itemId && d.PedidoId == pedidoId);

        if (item == null)
        {
            return NotFound(new { mensaje = "Ítem del pedido no encontrado." });
        }

        // Validar que Cocina no modifique ítems de Bar y viceversa
        if (User.IsInRole(Roles.AdminCocina) && !User.IsInRole(Roles.Admin) && item.Area != "Cocina")
        {
            return Forbid();
        }
        if (User.IsInRole(Roles.AdminBar) && !User.IsInRole(Roles.Admin) && item.Area != "Bar")
        {
            return Forbid();
        }

        item.EstadoItem = dto.EstadoItem;

        // Si todos los ítems están listos, marcar el pedido como Listo para que el mesero lo entregue
        var pedido = item.Pedido!;
        var itemsPedido = await _context.DetallesPedido.Where(d => d.PedidoId == pedidoId).ToListAsync();
        if (itemsPedido.All(i => i.EstadoItem == "Listo"))
        {
            pedido.Estado = "Listo";
        }
        else if (itemsPedido.Any(i => i.EstadoItem == "EnPreparacion" || i.EstadoItem == "Listo"))
        {
            pedido.Estado = "EnPreparacion";
        }

        await _context.SaveChangesAsync();
        return Ok(new { mensaje = $"Ítem '{item.ProductoNombre}' actualizado a {dto.EstadoItem}.", estadoPedido = pedido.Estado });
    }

    /// <summary>
    /// El Mesero o Administrador marca el pedido como "Entregado" a la mesa.
    /// </summary>
    [HttpPatch("{pedidoId}/entregar")]
    [Authorize(Roles = $"{Roles.Mesero},{Roles.Admin}")]
    public async Task<ActionResult> EntregarPedido(int pedidoId)
    {
        var pedido = await _context.Pedidos.FindAsync(pedidoId);
        if (pedido == null)
        {
            return NotFound(new { mensaje = "Pedido no encontrado." });
        }

        pedido.Estado = "Entregado";
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = $"Pedido #{pedidoId} entregado a la mesa {pedido.NumeroMesa} con éxito." });
    }

    /// <summary>
    /// HU-12: Cancelación de pedido. Solo si aún está "Pendiente".
    /// </summary>
    [HttpDelete("{pedidoId}")]
    [Authorize(Roles = $"{Roles.Mesero},{Roles.Admin}")]
    public async Task<ActionResult> CancelarPedido(int pedidoId)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Detalles)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido == null)
        {
            return NotFound(new { mensaje = "Pedido no encontrado." });
        }

        if (pedido.Estado != "Pendiente")
        {
            return BadRequest(new { mensaje = "No se puede cancelar un pedido que ya está en preparación o listo." });
        }

        pedido.Estado = "Cancelado";
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = $"Pedido #{pedidoId} cancelado correctamente." });
    }
}
