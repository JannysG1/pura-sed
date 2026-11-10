using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Controllers;

public record LoginRequest(string Usuario, string Password);
public record RegisterRequest(string Usuario, string Email, string Password, string NombreCompleto);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Usuario)
            ?? await _userManager.FindByEmailAsync(request.Usuario);

        if (user == null)
        {
            return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return Unauthorized(new { mensaje = "Cuenta bloqueada temporalmente. Intenta más tarde." });
        }

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            await _userManager.AccessFailedAsync(user);
            return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var rol in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, rol));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expira = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expira,
            signingCredentials: creds);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expira,
            usuario = user.UserName,
            roles
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Usuario,
            Email = request.Email,
            NombreCompleto = request.NombreCompleto
        };

        var resultado = await _userManager.CreateAsync(user, request.Password);
        if (!resultado.Succeeded)
        {
            return BadRequest(new { errores = resultado.Errors.Select(e => e.Description) });
        }

        // El rol lo decide el servidor, nunca el cliente
        // En Pura Sed los clientes registrados obtienen el rol "Cliente" y "Usuario"
        await _userManager.AddToRoleAsync(user, "Cliente");
        await _userManager.AddToRoleAsync(user, "Usuario");

        return Ok(new { mensaje = "Usuario registrado correctamente", usuario = user.UserName, roles = new[] { "Cliente", "Usuario" } });
    }
}
