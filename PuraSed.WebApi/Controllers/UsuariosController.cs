using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuraSed.WebApi.Constants;
using PuraSed.WebApi.DTOs;
using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Controllers;

[Authorize(Roles = Roles.SuperAdmin)]
[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsuariosController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// HU-03: Listar usuarios del personal y sus roles.
    /// Solo permitido al Superadministrador.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> ObtenerUsuarios()
    {
        var users = await _userManager.Users.ToListAsync();
        var lista = new List<object>();

        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            lista.Add(new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.NombreCompleto,
                u.LockoutEnabled,
                Roles = roles
            });
        }

        return Ok(lista);
    }

    /// <summary>
    /// HU-03: Crear usuario del personal y asignarle su rol de manera controlada.
    /// </summary>
    [HttpPost("personal")]
    public async Task<ActionResult> CrearUsuarioPersonal([FromBody] CrearUsuarioPersonalDTO dto)
    {
        if (!Roles.Todos.Contains(dto.Rol))
        {
            return BadRequest(new { mensaje = $"El rol '{dto.Rol}' no es válido en el sistema." });
        }

        var user = new ApplicationUser
        {
            UserName = dto.Usuario,
            Email = dto.Email,
            NombreCompleto = dto.NombreCompleto,
            EmailConfirmed = true
        };

        var resultado = await _userManager.CreateAsync(user, dto.Password);
        if (!resultado.Succeeded)
        {
            return BadRequest(new { errores = resultado.Errors.Select(e => e.Description) });
        }

        await _userManager.AddToRoleAsync(user, dto.Rol);

        return Ok(new
        {
            mensaje = $"Usuario {dto.Usuario} creado exitosamente con el rol {dto.Rol}.",
            usuarioId = user.Id,
            rol = dto.Rol
        });
    }

    /// <summary>
    /// HU-03: Asignar un rol adicional a un usuario.
    /// </summary>
    [HttpPost("asignar-rol")]
    public async Task<ActionResult> AsignarRol([FromBody] AsignarRolUsuarioDTO dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UsuarioIdOEmail)
            ?? await _userManager.FindByEmailAsync(dto.UsuarioIdOEmail)
            ?? await _userManager.FindByNameAsync(dto.UsuarioIdOEmail);

        if (user == null)
        {
            return NotFound(new { mensaje = "Usuario no encontrado." });
        }

        if (!await _roleManager.RoleExistsAsync(dto.Rol))
        {
            return BadRequest(new { mensaje = $"El rol '{dto.Rol}' no existe en la base de datos." });
        }

        if (await _userManager.IsInRoleAsync(user, dto.Rol))
        {
            return BadRequest(new { mensaje = $"El usuario ya tiene asignado el rol '{dto.Rol}'." });
        }

        var resultado = await _userManager.AddToRoleAsync(user, dto.Rol);
        if (!resultado.Succeeded)
        {
            return BadRequest(new { errores = resultado.Errors.Select(e => e.Description) });
        }

        return Ok(new { mensaje = $"Rol '{dto.Rol}' asignado correctamente a '{user.UserName}'." });
    }
}
