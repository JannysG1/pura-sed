namespace PuraSed.WebApi.DTOs;

public class ProductoCreateDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    public bool Activo { get; set; } = true;
}

public class ProductoUpdateDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    public bool Activo { get; set; } = true;
}
