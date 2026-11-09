namespace PuraSed.WebApi.DTOs;

public class ProductoCreateDTO
{
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public bool Activo { get; set; }
}

public class ProductoUpdateDTO
{
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public bool Activo { get; set; }
}
