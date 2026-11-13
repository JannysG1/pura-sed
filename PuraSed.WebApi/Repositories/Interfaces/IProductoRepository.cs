using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Repositories.Interfaces;

public interface IProductoRepository
{
    Task<List<Producto>> ObtenerTodosAsync();

    Task<List<Producto>> ObtenerPorCategoriaAsync(string categoria);

    Task<Producto?> BuscarPorIdAsync(int id);

    Task<Producto> AgregarAsync(Producto producto);

    Task<bool> ActualizarAsync(Producto producto);

    Task<bool> EliminarPorIdAsync(int id);

    Task<bool> CambiarDisponibilidadAsync(int id, bool activo);
}