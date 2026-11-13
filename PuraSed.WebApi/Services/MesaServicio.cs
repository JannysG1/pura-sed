using PuraSed.WebApi.Models;
using PuraSed.WebApi.Repositories.Interfaces;
using PuraSed.WebApi.Services.Interfaces;

namespace PuraSed.WebApi.Services;

public class MesaServicio : IMesaServicio
{
    private readonly IMesaRepository _repo;

    public MesaServicio(IMesaRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<Mesa>> ObtenerTodasAsync()
    {
        return await _repo.ObtenerTodasAsync();
    }

    public async Task<Mesa?> BuscarPorNumeroAsync(int numero)
    {
        return await _repo.BuscarPorNumeroAsync(numero);
    }

    public async Task<bool> ActualizarEstadoAsync(
     int numero,
     string estado,
     string? meseroAsignadoId,
     string? meseroAsignadoNombre)
    {
        string[] estadosPermitidos =
        {
        "Disponible",
        "Ocupada",
        "Reservada",
        "EnMantenimiento"
    };

        if (!estadosPermitidos.Contains(estado))
        {
            throw new ArgumentException(
                "Estado no válido. Usa Disponible, Ocupada, " +
                "Reservada o EnMantenimiento.");
        }

        return await _repo.ActualizarEstadoAsync(
            numero,
            estado,
            meseroAsignadoId,
            meseroAsignadoNombre);
    }
}