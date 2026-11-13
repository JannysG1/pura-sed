namespace PuraSed.WebApi.Models;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    public bool Activo { get; set; } = true;
}
