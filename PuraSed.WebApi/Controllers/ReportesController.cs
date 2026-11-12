using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuraSed.WebApi.Constants;
using PuraSed.WebApi.Data;

namespace PuraSed.WebApi.Controllers;

[Authorize(Roles = Roles.Administracion)]
[Route("api/[controller]")]
[ApiController]
public class ReportesController : ControllerBase
{
    private readonly PuraSedDbContext _context;

    public ReportesController(PuraSedDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// HU-27: Reporte de ventas diarias.
    /// Solo accesible para Administrador y Superadministrador.
    /// </summary>
    [HttpGet("ventas-diarias")]
    public async Task<ActionResult> VentasDiarias([FromQuery] DateTime? fechaInicio = null, [FromQuery] DateTime? fechaFin = null)
    {
        var inicio = fechaInicio?.ToUniversalTime() ?? DateTime.UtcNow.Date;
        var fin = fechaFin?.ToUniversalTime() ?? DateTime.UtcNow.Date.AddDays(1);

        var pedidosPagados = await _context.Pedidos
            .Where(p => p.Pagado && p.FechaHora >= inicio && p.FechaHora <= fin)
            .ToListAsync();

        var totalVendido = pedidosPagados.Sum(p => p.Total);
        var cantidadCuentas = pedidosPagados.Count;

        return Ok(new
        {
            periodo = $"{inicio:yyyy-MM-dd} al {fin:yyyy-MM-dd}",
            totalVendido,
            cantidadCuentasPagadas = cantidadCuentas,
            ticketPromedio = cantidadCuentas > 0 ? (totalVendido / cantidadCuentas) : 0
        });
    }

    /// <summary>
    /// HU-28: Índice de pedidos por mesero.
    /// Permite evaluar el desempeño del personal de salón.
    /// </summary>
    [HttpGet("rendimiento-meseros")]
    public async Task<ActionResult> RendimientoMeseros()
    {
        var pedidos = await _context.Pedidos
            .Where(p => !string.IsNullOrEmpty(p.MeseroNombre))
            .ToListAsync();

        var agrupados = pedidos
            .GroupBy(p => p.MeseroNombre)
            .Select(g => new
            {
                Mesero = g.Key,
                TotalPedidosAtendidos = g.Count(),
                MontoTotalVendido = g.Sum(p => p.Total),
                PedidosPagados = g.Count(p => p.Pagado)
            })
            .OrderByDescending(x => x.TotalPedidosAtendidos)
            .ToList();

        return Ok(agrupados);
    }
}
