using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuraSed.WebApi.Constants;
using PuraSed.WebApi.Data;
using PuraSed.WebApi.DTOs;

namespace PuraSed.WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PagosController : ControllerBase
{
    private readonly PuraSedDbContext _context;

    public PagosController(PuraSedDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// HU-20: Habilitar el cobro de una mesa. Solo el Administrador o SuperAdmin puede habilitarlo.
    /// </summary>
    [HttpPost("habilitar/{numeroMesa}")]
    [Authorize(Roles = Roles.Administracion)]
    public async Task<ActionResult> HabilitarCobro(int numeroMesa)
    {
        var pedidos = await _context.Pedidos
            .Where(p => p.NumeroMesa == numeroMesa && !p.Pagado && p.Estado != "Cancelado")
            .ToListAsync();

        if (pedidos.Count == 0)
        {
            return NotFound(new { mensaje = $"No existen pedidos activos pendientes de cobro para la mesa {numeroMesa}." });
        }

        foreach (var p in pedidos)
        {
            p.CobroHabilitado = true;
        }

        await _context.SaveChangesAsync();
        decimal totalCuenta = pedidos.Sum(p => p.Total);

        return Ok(new
        {
            mensaje = $"Cobro habilitado exitosamente para la mesa {numeroMesa}.",
            numeroMesa,
            totalPagar = totalCuenta,
            pedidosIncluidos = pedidos.Select(p => p.Id).ToList()
        });
    }

    /// <summary>
    /// HU-23: Confirmar pago en efectivo.
    /// El Mesero (o Admin) recibe el dinero físico en mesa o caja y confirma el pago.
    /// </summary>
    [HttpPost("confirmar-efectivo")]
    [Authorize(Roles = Roles.GestionCobro)]
    public async Task<ActionResult> ConfirmarEfectivo([FromBody] ConfirmarPagoEfectivoDTO dto)
    {
        var pedidos = await _context.Pedidos
            .Where(p => p.NumeroMesa == dto.NumeroMesa && !p.Pagado && p.Estado != "Cancelado")
            .ToListAsync();

        if (pedidos.Count == 0)
        {
            return NotFound(new { mensaje = $"No hay consumos pendientes para la mesa {dto.NumeroMesa}." });
        }

        decimal total = pedidos.Sum(p => p.Total);
        if (dto.MontoRecibido < total)
        {
            return BadRequest(new { mensaje = $"El monto recibido (${dto.MontoRecibido}) es menor al total de la cuenta (${total})." });
        }

        foreach (var p in pedidos)
        {
            p.Pagado = true;
            p.FormaPago = "Efectivo";
        }

        // Liberar mesa
        var mesa = await _context.Mesas.FirstOrDefaultAsync(m => m.Numero == dto.NumeroMesa);
        if (mesa != null)
        {
            mesa.Estado = "Disponible";
            mesa.MeseroAsignadoId = null;
            mesa.MeseroAsignadoNombre = null;
        }

        await _context.SaveChangesAsync();

        decimal cambio = dto.MontoRecibido - total;
        return Ok(new
        {
            mensaje = $"Pago en efectivo confirmado para la mesa {dto.NumeroMesa}.",
            total,
            montoRecibido = dto.MontoRecibido,
            cambio,
            mesaLiberada = true
        });
    }

    /// <summary>
    /// HU-21: Pagar con tarjeta desde la app (Cliente en mesa).
    /// </summary>
    [HttpPost("tarjeta/{numeroMesa}")]
    [Authorize(Roles = $"{Roles.Cliente},{Roles.Admin}")]
    public async Task<ActionResult> PagarTarjeta(int numeroMesa)
    {
        var pedidos = await _context.Pedidos
            .Where(p => p.NumeroMesa == numeroMesa && !p.Pagado && p.Estado != "Cancelado")
            .ToListAsync();

        if (pedidos.Count == 0)
        {
            return NotFound(new { mensaje = "No hay cuenta pendiente para esta mesa." });
        }

        if (!pedidos.Any(p => p.CobroHabilitado))
        {
            return BadRequest(new { mensaje = "El cobro aún no ha sido habilitado por administración." });
        }

        foreach (var p in pedidos)
        {
            p.Pagado = true;
            p.FormaPago = "Tarjeta";
        }

        var mesa = await _context.Mesas.FirstOrDefaultAsync(m => m.Numero == numeroMesa);
        if (mesa != null)
        {
            mesa.Estado = "Disponible";
        }

        await _context.SaveChangesAsync();
        return Ok(new { mensaje = $"Pago con tarjeta aprobado para mesa {numeroMesa}.", total = pedidos.Sum(p => p.Total) });
    }
}
