using PuraSed.WebApi.DTOs;

namespace PuraSed.WebApi.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResultadoDTO> LoginAsync(LoginRequest request);

    Task<RegistroResultadoDTO> RegistrarAsync(RegisterRequest request);
}