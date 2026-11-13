using Microsoft.AspNetCore.Identity;
using PuraSed.WebApi.Constants;
using PuraSed.WebApi.DTOs;
using PuraSed.WebApi.Models;
using PuraSed.WebApi.Services.Interfaces;

namespace PuraSed.WebApi.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IValidacionUsuarioService _validacionUsuario;

    public AuthService(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    IValidacionUsuarioService validacionUsuario)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _validacionUsuario = validacionUsuario;
    }

    public async Task<LoginResultadoDTO> LoginAsync(LoginRequest request)
    {
        var validacion = await _validacionUsuario.ValidarAsync(
            request.Usuario,
            request.Password);

        if (!validacion.Exitoso || validacion.Usuario == null)
        {
            return new LoginResultadoDTO(
                false,
                validacion.Mensaje,
                null);
        }

        var user = validacion.Usuario;

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerarToken(user, roles);

        var datos = new LoginRespuestaDTO(
            token.Token,
            token.Expira,
            user.UserName ?? string.Empty,
            user.NombreCompleto ?? string.Empty,
            roles);

        return new LoginResultadoDTO(
            true,
            "Inicio de sesión correcto",
            datos);
    }
    public async Task<RegistroResultadoDTO> RegistrarAsync(
        RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Usuario,
            Email = request.Email,
            NombreCompleto = request.NombreCompleto
        };

        var resultado = await _userManager.CreateAsync(
            user, request.Password);

        if (!resultado.Succeeded)
        {
            return new RegistroResultadoDTO(
                Exitoso: false,
                UsuarioCreado: false,
                Mensaje: "No se pudo registrar el usuario.",
                Usuario: null,
                Roles: Array.Empty<string>(),
                Errores: resultado.Errors
                    .Select(e => e.Description)
                    .ToArray());
        }

        // El servidor establece los roles del nuevo usuario.
        var roles = new[] { Roles.Cliente, Roles.Usuario };

        var resultadoRoles = await _userManager.AddToRolesAsync(
            user, roles);

        if (!resultadoRoles.Succeeded)
        {
            return new RegistroResultadoDTO(
                Exitoso: false,
                UsuarioCreado: true,
                Mensaje: "El usuario se creó, pero no se pudieron asignar sus roles.",
                Usuario: user.UserName,
                Roles: Array.Empty<string>(),
                Errores: resultadoRoles.Errors
                    .Select(e => e.Description)
                    .ToArray());
        }

        return new RegistroResultadoDTO(
            Exitoso: true,
            UsuarioCreado: true,
            Mensaje: "Usuario registrado correctamente",
            Usuario: user.UserName,
            Roles: roles,
            Errores: Array.Empty<string>());
    }
}