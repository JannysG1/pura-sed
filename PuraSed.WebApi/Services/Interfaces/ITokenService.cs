using PuraSed.WebApi.DTOs;
using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Services.Interfaces;

public interface ITokenService
{
    TokenResultadoDTO GenerarToken(
        ApplicationUser usuario,
        IEnumerable<string> roles);
}