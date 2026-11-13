namespace PuraSed.WebApi.DTOs;

public record TokenResultadoDTO(
    string Token,
    DateTime Expira
);