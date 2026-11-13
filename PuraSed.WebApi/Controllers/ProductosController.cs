using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuraSed.WebApi.Constants;
using PuraSed.WebApi.DTOs;
using PuraSed.WebApi.Models;
using PuraSed.WebApi.Services.Interfaces;

namespace PuraSed.WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly IProductoServicio _service;

    public ProductosController(IProductoServicio service)
    {
        _service = service;
    }

    private bool DebeMostrarSoloDisponibles()
    {
        return User.Identity?.IsAuthenticated != true
            || User.IsInRole(Roles.Cliente);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Producto>>> ObtenerTodos()
    {
        var productos = await _service.ObtenerTodosAsync(
            DebeMostrarSoloDisponibles());

        return Ok(productos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Producto>> ObtenerPorId(int id)
    {
        var producto = await _service.BuscarPorIdAsync(
            id, DebeMostrarSoloDisponibles());

        if (producto == null)
        {
            return NotFound();
        }

        return Ok(producto);
    }

    [HttpGet("categoria/{categoria}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Producto>>> ObtenerPorCategoria(
        string categoria)
    {
        var productos = await _service.ObtenerPorCategoriaAsync(
            categoria, DebeMostrarSoloDisponibles());

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

        await _service.AgregarAsync(producto);
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

        bool actualizado = await _service.ActualizarAsync(producto);
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
        bool cambiado = await _service.CambiarDisponibilidadAsync(id, activo);
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
        bool eliminado = await _service.EliminarPorIdAsync(id);
        if (!eliminado)
        {
            return NotFound();
        }
        return NoContent();
    }
}
