using Microsoft.AspNetCore.Identity;
using PuraSed.WebApi.DTOs;
using PuraSed.WebApi.Models;
using PuraSed.WebApi.Services.Interfaces;

namespace PuraSed.WebApi.Services;

public class ValidacionUsuarioService : IValidacionUsuarioService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ValidacionUsuarioService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ValidacionUsuarioResultadoDTO> ValidarAsync(
        string usuario,
        string password)
    {
        if (string.IsNullOrWhiteSpace(usuario)
            || string.IsNullOrEmpty(password))
        {
            return new ValidacionUsuarioResultadoDTO(
                false,
                "Usuario o contraseña incorrectos",
                null);
        }

        var user = await _userManager.FindByNameAsync(usuario)
            ?? await _userManager.FindByEmailAsync(usuario);

        if (user == null)
        {
            return new ValidacionUsuarioResultadoDTO(
                false,
                "Usuario o contraseña incorrectos",
                null);
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return new ValidacionUsuarioResultadoDTO(
                false,
                "Cuenta bloqueada temporalmente. Intenta más tarde.",
                null);
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            await _userManager.AccessFailedAsync(user);

            return new ValidacionUsuarioResultadoDTO(
                false,
                "Usuario o contraseña incorrectos",
                null);
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        return new ValidacionUsuarioResultadoDTO(
            true,
            "Credenciales válidas",
            user);
    }
}