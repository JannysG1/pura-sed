namespace PuraSed.WebApi.Models;

public class Pedido
{
    public int Id { get; set; }

    /// <summary>Mesa física donde se sirve el pedido</summary>
    public int NumeroMesa { get; set; }

    public DateTime FechaHora { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Estado global del pedido:
    /// "Pendiente", "EnPreparacion", "Listo", "Entregado", "Cancelado"
    /// </summary>
    public string Estado { get; set; } = "Pendiente";

    public decimal Total { get; set; }

    public string? Observaciones { get; set; }

    /// <summary>
    /// ID del mesero que registró o atiende la mesa (HU-13, HU-28 Índice de pedidos por mesero).
    /// </summary>
    public string? MeseroId { get; set; }

    public string? MeseroNombre { get; set; }

    /// <summary>
    /// Usuario o sesión del cliente que solicitó el pedido desde la mesa.
    /// </summary>
    public string? ClienteUsuario { get; set; }

    /// <summary>HU-20: Habilitar el cobro por parte del Administrador</summary>
    public bool CobroHabilitado { get; set; } = false;

    /// <summary>Indica si la cuenta del pedido ya fue cancelada</summary>
    public bool Pagado { get; set; } = false;

    /// <summary>Forma de pago elegida: "Tarjeta", "Transferencia", "Efectivo"</summary>
    public string? FormaPago { get; set; }

    public List<DetallePedido> Detalles { get; set; } = new();
}
