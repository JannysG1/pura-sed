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
    public DbSet<Mesa> Mesas => Set<Mesa>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallesPedido => Set<DetallePedido>();

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

        modelBuilder.Entity<Mesa>(entity =>
        {
            entity.HasIndex(m => m.Numero).IsUnique();
            entity.Property(m => m.Estado).HasMaxLength(50).IsRequired();
            entity.Property(m => m.MeseroAsignadoNombre).HasMaxLength(120);
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.Property(p => p.Total).HasPrecision(18, 2);
            entity.Property(p => p.Estado).HasMaxLength(50).IsRequired();
            entity.Property(p => p.Observaciones).HasMaxLength(500);
            entity.Property(p => p.MeseroNombre).HasMaxLength(120);
            entity.Property(p => p.ClienteUsuario).HasMaxLength(120);
            entity.Property(p => p.FormaPago).HasMaxLength(50);

            entity.HasMany(p => p.Detalles)
                  .WithOne(d => d.Pedido)
                  .HasForeignKey(d => d.PedidoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.Property(d => d.ProductoNombre).HasMaxLength(120).IsRequired();
            entity.Property(d => d.Area).HasMaxLength(50).IsRequired();
            entity.Property(d => d.EstadoItem).HasMaxLength(50).IsRequired();
            entity.Property(d => d.PrecioUnitario).HasPrecision(18, 2);
            entity.Property(d => d.Subtotal).HasPrecision(18, 2);
            entity.Property(d => d.Observaciones).HasMaxLength(300);
        });
    }
}
