using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Services.Interfaces;

public interface IMesaServicio
{
    Task<List<Mesa>> ObtenerTodasAsync();

    Task<Mesa?> BuscarPorNumeroAsync(int numero);

    Task<bool> ActualizarEstadoAsync(
        int numero,
        string estado,
        string? meseroAsignadoId,
        string? meseroAsignadoNombre);
}