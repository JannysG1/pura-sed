using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuraSed.WebApi.Constants;
using PuraSed.WebApi.DTOs;
using PuraSed.WebApi.Models;
using PuraSed.WebApi.Repositories;

namespace PuraSed.WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly ProductoRepository _repo;

    public ProductosController(ProductoRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Producto>>> ObtenerTodos()
    {
        var productos = await _repo.ObtenerTodosAsync();

        // HU-10: Si el usuario es un cliente o no está autenticado, solo mostramos los productos disponibles (no agotados)
        if (User.Identity == null || !User.Identity.IsAuthenticated || User.IsInRole(Roles.Cliente))
        {
            return Ok(productos.Where(p => p.Activo).ToList());
        }

        // Para el personal (Admin, Mesero, Cocina, Bar), se devuelven todos para que puedan ver los agotados sombreados
        return Ok(productos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Producto>> ObtenerPorId(int id)
    {
        var producto = await _repo.BuscarPorIdAsync(id);
        if (producto == null)
        {
            return NotFound();
        }
        return Ok(producto);
    }

    [HttpGet("categoria/{categoria}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Producto>>> ObtenerPorCategoria(string categoria)
    {
        var productos = await _repo.ObtenerPorCategoriaAsync(categoria);
        if (User.Identity == null || !User.Identity.IsAuthenticated || User.IsInRole(Roles.Cliente))
        {
            return Ok(productos.Where(p => p.Activo).ToList());
        }
        return Ok(productos);
    }

    [HttpPost]
    [Authorize(Roles = Roles.GestionMenu)]
    public async Task<ActionResult> Agregar([FromBody] ProductoCreateDTO dto)
    {
        var producto = new Producto
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            Categoria = dto.Categoria,
            ImagenUrl = dto.ImagenUrl,
            Activo = dto.Activo
        };

        await _repo.AgregarAsync(producto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.GestionMenu)]
    public async Task<ActionResult> Actualizar(int id, [FromBody] ProductoUpdateDTO dto)
    {
        var producto = new Producto
        {
            Id = id,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            Categoria = dto.Categoria,
            ImagenUrl = dto.ImagenUrl,
            Activo = dto.Activo
        };

        bool actualizado = await _repo.ActualizarAsync(producto);
        if (!actualizado)
        {
            return NotFound();
        }
        return NoContent();
    }

    /// <summary>
    /// HU-10: Permite al Administrador, Cocina y Bar cambiar la disponibilidad (agotado/activo).
    /// </summary>
    [HttpPatch("{id}/disponibilidad")]
    [Authorize(Roles = Roles.GestionInventario)]
    public async Task<ActionResult> CambiarDisponibilidad(int id, [FromQuery] bool activo)
    {
        bool cambiado = await _repo.CambiarDisponibilidadAsync(id, activo);
        if (!cambiado)
        {
            return NotFound(new { mensaje = "Producto no encontrado." });
        }
        return Ok(new { id, activo, mensaje = activo ? "Producto marcado como disponible" : "Producto marcado como agotado" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.GestionMenu)]
    public async Task<ActionResult> Eliminar(int id)
    {
        bool eliminado = await _repo.EliminarPorIdAsync(id);
        if (!eliminado)
        {
            return NotFound();
        }
        return NoContent();
    }
}
