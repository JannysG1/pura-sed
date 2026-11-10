using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<List<Producto>>> ObtenerTodos()
    {
        var productos = await _repo.ObtenerTodosAsync();
        return Ok(productos);
    }

    [HttpGet("{id}")]
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
    public async Task<ActionResult<List<Producto>>> ObtenerPorCategoria(string categoria)
    {
        var productos = await _repo.ObtenerPorCategoriaAsync(categoria);
        return Ok(productos);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin,Mesero")]
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

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
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
