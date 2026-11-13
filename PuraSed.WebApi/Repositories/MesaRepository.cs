using Microsoft.EntityFrameworkCore;
using PuraSed.WebApi.Data;
using PuraSed.WebApi.Models;
using PuraSed.WebApi.Repositories.Interfaces;

namespace PuraSed.WebApi.Repositories;

public class MesaRepository : IMesaRepository
{
    private readonly PuraSedDbContext _context;

    public MesaRepository(PuraSedDbContext context)
    {
        _context = context;
    }

    public async Task<List<Mesa>> ObtenerTodasAsync()
    {
        return await _context.Mesas
            .AsNoTracking()
            .OrderBy(m => m.Numero)
            .ToListAsync();
    }

    public async Task<Mesa?> BuscarPorNumeroAsync(int numero)
    {
        return await _context.Mesas
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Numero == numero);
    }

    public async Task<bool> ActualizarEstadoAsync(
        int numero,
        string estado,
        string? meseroAsignadoId,
        string? meseroAsignadoNombre)
    {
        var mesa = await _context.Mesas
            .FirstOrDefaultAsync(m => m.Numero == numero);

        if (mesa == null)
        {
            return false;
        }

        mesa.Estado = estado;

        if (!string.IsNullOrEmpty(meseroAsignadoId))
        {
            mesa.MeseroAsignadoId = meseroAsignadoId;
        }

        if (!string.IsNullOrEmpty(meseroAsignadoNombre))
        {
            mesa.MeseroAsignadoNombre = meseroAsignadoNombre;
        }

        await _context.SaveChangesAsync();

        return true;
    }
}