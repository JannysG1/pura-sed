using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Services.Interfaces;

public interface IProductoServicio
{
    Task<List<Producto>> ObtenerTodosAsync(bool soloDisponibles);

    Task<List<Producto>> ObtenerPorCategoriaAsync(
        string categoria, bool soloDisponibles);

    Task<Producto?> BuscarPorIdAsync(int id, bool soloDisponibles);
    Task<Producto> AgregarAsync(Producto producto);

    Task<bool> ActualizarAsync(Producto producto);

    Task<bool> EliminarPorIdAsync(int id);

    Task<bool> CambiarDisponibilidadAsync(int id, bool activo);
}