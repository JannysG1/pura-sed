using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuraSed.WebApi.DTOs;
using PuraSed.WebApi.Services.Interfaces;

namespace PuraSed.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        var resultado = await _authService.LoginAsync(request);

        if (!resultado.Exitoso)
        {
            return Unauthorized(new
            {
                mensaje = resultado.Mensaje
            });
        }

        return Ok(resultado.Datos);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        var resultado = await _authService.RegistrarAsync(request);

        if (!resultado.Exitoso)
        {
            if (resultado.UsuarioCreado)
            {
                return StatusCode(500, new
                {
                    mensaje = resultado.Mensaje
                });
            }

            return BadRequest(new
            {
                mensaje = resultado.Mensaje,
                errores = resultado.Errores
            });
        }

        return Ok(new
        {
            mensaje = resultado.Mensaje,
            usuario = resultado.Usuario,
            roles = resultado.Roles
        });
    }
}