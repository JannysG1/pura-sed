using Microsoft.EntityFrameworkCore;
using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Data;

public static class MesaSeeder
{
    public static async Task SeedAsync(PuraSedDbContext context)
    {
        if (await context.Mesas.AnyAsync())
        {
            return;
        }

        var mesas = new List<Mesa>
        {
            new Mesa { Numero = 1, Capacidad = 4, Estado = "Disponible", MeseroAsignadoNombre = "Mesero Salón Principal" },
            new Mesa { Numero = 2, Capacidad = 4, Estado = "Disponible", MeseroAsignadoNombre = "Mesero Salón Principal" },
            new Mesa { Numero = 3, Capacidad = 2, Estado = "Disponible", MeseroAsignadoNombre = "Mesero Salón Principal" },
            new Mesa { Numero = 4, Capacidad = 6, Estado = "Disponible", MeseroAsignadoNombre = "Mesero Salón Principal" },
            new Mesa { Numero = 5, Capacidad = 4, Estado = "Reservada", MeseroAsignadoNombre = "Mesero Salón Principal" },
            new Mesa { Numero = 6, Capacidad = 8, Estado = "Disponible", MeseroAsignadoNombre = "Mesero Salón Principal" },
            new Mesa { Numero = 7, Capacidad = 2, Estado = "Disponible", MeseroAsignadoNombre = "Mesero Salón Principal" },
            new Mesa { Numero = 8, Capacidad = 4, Estado = "Disponible", MeseroAsignadoNombre = "Mesero Salón Principal" }
        };

        await context.Mesas.AddRangeAsync(mesas);
        await context.SaveChangesAsync();
    }
}
