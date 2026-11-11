using Microsoft.AspNetCore.Authorization;
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
public class MesasController : ControllerBase
{
    private readonly PuraSedDbContext _context;

    public MesasController(PuraSedDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Consulta el estado de todas las mesas del local.
    /// Permitido a: Todo el personal operativo (Mesero, Asistente, Admin, SuperAdmin).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = Roles.OperativoMesas)]
    public async Task<ActionResult<List<Mesa>>> ObtenerTodas()
    {
        var mesas = await _context.Mesas.OrderBy(m => m.Numero).ToListAsync();
        return Ok(mesas);
    }

    /// <summary>
    /// Consulta el estado de una mesa por número.
    /// </summary>
    [HttpGet("{numero}")]
    [Authorize(Roles = Roles.OperativoMesas)]
    public async Task<ActionResult<Mesa>> ObtenerPorNumero(int numero)
    {
        var mesa = await _context.Mesas.FirstOrDefaultAsync(m => m.Numero == numero);
        if (mesa == null)
        {
            return NotFound(new { mensaje = $"Mesa {numero} no encontrada." });
        }
        return Ok(mesa);
    }

    /// <summary>
    /// Actualiza el estado de la mesa (Disponible, Ocupada, Reservada) y mesero asignado.
    /// </summary>
    [HttpPatch("{numero}/estado")]
    [Authorize(Roles = Roles.OperativoMesas)]
    public async Task<ActionResult> ActualizarEstado(int numero, [FromBody] ActualizarEstadoMesaDTO dto)
    {
        var mesa = await _context.Mesas.FirstOrDefaultAsync(m => m.Numero == numero);
        if (mesa == null)
        {
            return NotFound(new { mensaje = $"Mesa {numero} no encontrada." });
        }

        mesa.Estado = dto.Estado;
        if (!string.IsNullOrEmpty(dto.MeseroAsignadoId))
        {
            mesa.MeseroAsignadoId = dto.MeseroAsignadoId;
        }
        if (!string.IsNullOrEmpty(dto.MeseroAsignadoNombre))
        {
            mesa.MeseroAsignadoNombre = dto.MeseroAsignadoNombre;
        }

        await _context.SaveChangesAsync();
        return Ok(new { mensaje = $"Mesa {numero} actualizada a '{dto.Estado}'.", mesa });
    }
}
