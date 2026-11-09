using Microsoft.EntityFrameworkCore;
using PuraSed.WebApi.Data;
using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Repositories;

public class ProductoRepository
{
    private readonly CafeteriaDbContext _context;

    public ProductoRepository(CafeteriaDbContext context)
    {
        _context = context;
    }

    public async Task<List<Producto>> ObtenerTodosAsync()
    {
        return await _context.Productos.AsNoTracking().ToListAsync();
    }

    public async Task<Producto?> BuscarPorIdAsync(int id)
    {
        return await _context.Productos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Producto> AgregarAsync(Producto producto)
    {
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
        return producto;
    }

    public async Task<bool> ActualizarAsync(Producto producto)
    {
        var existente = await _context.Productos.FindAsync(producto.Id);
        if (existente == null)
        {
            return false;
        }

        existente.Nombre = producto.Nombre;
        existente.Precio = producto.Precio;
        existente.Activo = producto.Activo;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarPorIdAsync(int id)
    {
        var existente = await _context.Productos.FindAsync(id);
        if (existente == null)
        {
            return false;
        }

        _context.Productos.Remove(existente);
        await _context.SaveChangesAsync();
        return true;
    }
}
