namespace PuraSed.WebApi.DTOs;

public record ItemPedidoCreateDTO(
    int ProductoId,
    int Cantidad,
    string? Observaciones
);

public record CrearPedidoDTO(
    int NumeroMesa,
    string? Observaciones,
    List<ItemPedidoCreateDTO> Items
);

public record ActualizarEstadoItemDTO(
    string EstadoItem // "EnPreparacion", "Listo"
);

public record ActualizarEstadoPedidoDTO(
    string Estado // "Pendiente", "EnPreparacion", "Listo", "Entregado", "Cancelado"
);

public record ActualizarEstadoMesaDTO(
    string Estado, // "Disponible", "Ocupada", "Reservada", "EnMantenimiento"
    string? MeseroAsignadoId,
    string? MeseroAsignadoNombre
);

public record ConfirmarPagoEfectivoDTO(
    int NumeroMesa,
    decimal MontoRecibido,
    string? Observaciones
);

public record AsignarRolUsuarioDTO(
    string UsuarioIdOEmail,
    string Rol
);

public record CrearUsuarioPersonalDTO(
    string Usuario,
    string Email,
    string Password,
    string NombreCompleto,
    string Rol
);
