using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Data;

public class CafeteriaDbContext : IdentityDbContext<ApplicationUser>
{
    public CafeteriaDbContext(DbContextOptions<CafeteriaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // NUEVA: siempre primero

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.Property(p => p.Nombre).HasMaxLength(100);
            entity.Property(p => p.Precio).HasPrecision(18, 2);
        });
    }
}
