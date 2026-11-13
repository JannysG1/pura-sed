using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PuraSed.WebApi.DTOs;
using PuraSed.WebApi.Models;
using PuraSed.WebApi.Services.Interfaces;

namespace PuraSed.WebApi.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public TokenResultadoDTO GenerarToken(
        ApplicationUser usuario,
        IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id),

            new Claim(
                ClaimTypes.Name,
                usuario.UserName ?? string.Empty),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        foreach (var rol in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, rol));
        }

        var clave = _config["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "No se ha configurado Jwt:Key.");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(clave));

        var credenciales = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expira = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expira,
            signingCredentials: credenciales);

        var tokenTexto = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return new TokenResultadoDTO(tokenTexto, expira);
    }
}