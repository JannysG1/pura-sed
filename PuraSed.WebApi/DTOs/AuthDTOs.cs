namespace PuraSed.WebApi.DTOs;

public record LoginRequest(
    string Usuario,
    string Password);

public record RegisterRequest(
    string Usuario,
    string Email,
    string Password,
    string NombreCompleto);

public record LoginRespuestaDTO(
    string Token,
    DateTime Expira,
    string Usuario,
    string NombreCompleto,
    IEnumerable<string> Roles);

public record LoginResultadoDTO(
    bool Exitoso,
    string Mensaje,
    LoginRespuestaDTO? Datos);

public record RegistroResultadoDTO(
    bool Exitoso,
    bool UsuarioCreado,
    string Mensaje,
    string? Usuario,
    IEnumerable<string> Roles,
    IEnumerable<string> Errores);