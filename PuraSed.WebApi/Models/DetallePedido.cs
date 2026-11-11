using System.Text.Json.Serialization;

namespace PuraSed.WebApi.Models;

public class DetallePedido
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    [JsonIgnore]
    public Pedido? Pedido { get; set; }

    public int ProductoId { get; set; }

    public string ProductoNombre { get; set; } = string.Empty;

    /// <summary>
    /// Área de preparación de la comanda: "Cocina" o "Bar" (HU-14 Distribución automática).
    /// </summary>
    public string Area { get; set; } = "Cocina";

    public int Cantidad { get; set; } = 1;

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    /// <summary>
    /// Estado individual del ítem: "Pendiente", "EnPreparacion", "Listo" (HU-15 Gestión de pedidos de mi área).
    /// </summary>
    public string EstadoItem { get; set; } = "Pendiente";

    public string? Observaciones { get; set; }
}
