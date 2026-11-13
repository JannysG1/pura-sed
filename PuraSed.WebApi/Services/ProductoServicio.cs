using PuraSed.WebApi.Models;
using PuraSed.WebApi.Repositories.Interfaces;
using PuraSed.WebApi.Services.Interfaces;
namespace PuraSed.WebApi.Services
{
    public class ProductoServicio:IProductoServicio
    {
        private readonly IProductoRepository _repo;

        public ProductoServicio(IProductoRepository repo) { 
         _repo= repo;
        }


        private static bool PuedeMostrarse(
            Producto producto, bool soloDisponibles)
        {
            return !soloDisponibles || producto.Activo;
        }

        public async Task<List<Producto>> ObtenerTodosAsync(
            bool soloDisponibles)
        {
            var productos = await _repo.ObtenerTodosAsync();

            return productos
                .Where(p => PuedeMostrarse(p, soloDisponibles))
                .ToList();
        }

        public async Task<List<Producto>> ObtenerPorCategoriaAsync(
            string categoria, bool soloDisponibles)
        {
            var productos = await _repo.ObtenerPorCategoriaAsync(categoria);

            return productos
                .Where(p => PuedeMostrarse(p, soloDisponibles))
                .ToList();
        }

        public async Task<Producto?> BuscarPorIdAsync(
            int id, bool soloDisponibles)
        {
            var producto = await _repo.BuscarPorIdAsync(id);

            if (producto == null || !PuedeMostrarse(producto, soloDisponibles))
            {
                return null;
            }

            return producto;
        }

        public async Task<Producto> AgregarAsync(Producto producto) {
            return await _repo.AgregarAsync(producto);
        }

        public async Task<bool> ActualizarAsync(Producto producto) { 
            return await _repo.ActualizarAsync(producto);   
        }

        public async Task<bool> EliminarPorIdAsync(int id) {
            return await _repo.EliminarPorIdAsync(id);
        }

        public async Task<bool> CambiarDisponibilidadAsync(int id, bool activo) {
            return await _repo.CambiarDisponibilidadAsync(id, activo);
        }
    }
}
