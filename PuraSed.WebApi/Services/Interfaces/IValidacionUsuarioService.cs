using PuraSed.WebApi.DTOs;

namespace PuraSed.WebApi.Services.Interfaces;

public interface IValidacionUsuarioService
{
    Task<ValidacionUsuarioResultadoDTO> ValidarAsync(
        string usuario,
        string password);
}