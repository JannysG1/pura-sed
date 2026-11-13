using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuraSed.WebApi.Constants;
using PuraSed.WebApi.DTOs;
using PuraSed.WebApi.Models;
using PuraSed.WebApi.Services.Interfaces;

namespace PuraSed.WebApi.Controllers;

[Authorize(Roles = Roles.OperativoMesas)]
[Route("api/[controller]")]
[ApiController]
public class MesasController : ControllerBase
{
    private readonly IMesaServicio _servicio;

    public MesasController(IMesaServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<List<Mesa>>> ObtenerTodas()
    {
        var mesas = await _servicio.ObtenerTodasAsync();

        return Ok(mesas);
    }

    [HttpGet("{numero}")]
    public async Task<ActionResult<Mesa>> ObtenerPorNumero(int numero)
    {
        var mesa = await _servicio.BuscarPorNumeroAsync(numero);

        if (mesa == null)
        {
            return NotFound(new
            {
                mensaje = $"Mesa {numero} no encontrada."
            });
        }

        return Ok(mesa);
    }

    [HttpPatch("{numero}/estado")]
    public async Task<ActionResult> ActualizarEstado(
        int numero,
        [FromBody] ActualizarEstadoMesaDTO dto)
    {
        try
        {
            bool actualizado = await _servicio.ActualizarEstadoAsync(
                numero,
                dto.Estado,
                dto.MeseroAsignadoId,
                dto.MeseroAsignadoNombre);

            if (!actualizado)
            {
                return NotFound(new
                {
                    mensaje = $"Mesa {numero} no encontrada."
                });
            }

            var mesa = await _servicio.BuscarPorNumeroAsync(numero);

            return Ok(new
            {
                mensaje = $"Mesa {numero} actualizada.",
                mesa
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensaje = ex.Message
            });
        }
    }
}