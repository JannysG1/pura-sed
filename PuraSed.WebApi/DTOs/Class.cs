using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.DTOs;

public record ValidacionUsuarioResultadoDTO(
    bool Exitoso,
    string Mensaje,
    ApplicationUser? Usuario);