using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Data;

public class PuraSedDbContext : IdentityDbContext<ApplicationUser>
{
    public PuraSedDbContext(DbContextOptions<PuraSedDbContext> options)
        : base(options)
    {
    }

    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.Property(p => p.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(p => p.Descripcion).HasMaxLength(500);
            entity.Property(p => p.Categoria).HasMaxLength(80);
            entity.Property(p => p.ImagenUrl).HasMaxLength(300);
            entity.Property(p => p.Precio).HasPrecision(18, 2);
        });
    }
}
