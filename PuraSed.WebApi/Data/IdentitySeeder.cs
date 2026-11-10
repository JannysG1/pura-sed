using Microsoft.AspNetCore.Identity;
using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Data;

public static class IdentitySeeder
{
    // Roles del ecosistema Pura Sed según diseño y requerimientos de la Práctica 03:
    // Admin (Gerencia/Administrador), Mesero (Operativo de salón), Cliente (Consumidor final)
    private static readonly string[] Roles = { "Admin", "Mesero", "Cliente", "Usuario" };

    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // 1. Crear los roles si no existen
        foreach (var rol in Roles)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole(rol));
            }
        }

        // 2. Sembrado de Administrador (Admin)
        var adminEmail = config["SeedAdmin:Email"] ?? "admin@purased.com";
        var adminPassword = config["SeedAdmin:Password"] ?? "Admin123";

        var admin = await userManager.FindByEmailAsync(adminEmail) ?? await userManager.FindByNameAsync("admin");
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                NombreCompleto = "Administrador Pura Sed"
            };

            var resultado = await userManager.CreateAsync(admin, adminPassword);
            if (!resultado.Succeeded)
            {
                var errores = string.Join(", ", resultado.Errors.Select(e => e.Description));
                throw new InvalidOperationException("No se pudo crear el usuario Admin: " + errores);
            }
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // 3. Sembrado de Mesero (Mesero)
        var mesero = await userManager.FindByEmailAsync("mesero@purased.com") ?? await userManager.FindByNameAsync("mesero");
        if (mesero == null)
        {
            mesero = new ApplicationUser
            {
                UserName = "mesero",
                Email = "mesero@purased.com",
                NombreCompleto = "Mesero de Salón Pura Sed"
            };

            var resultadoMesero = await userManager.CreateAsync(mesero, "Mesero123");
            if (resultadoMesero.Succeeded)
            {
                await userManager.AddToRoleAsync(mesero, "Mesero");
            }
        }
        else if (!await userManager.IsInRoleAsync(mesero, "Mesero"))
        {
            await userManager.AddToRoleAsync(mesero, "Mesero");
        }

        // 4. Sembrado de Cliente (Cliente / Usuario)
        var cliente = await userManager.FindByEmailAsync("cliente@purased.com") ?? await userManager.FindByNameAsync("cliente");
        if (cliente == null)
        {
            cliente = new ApplicationUser
            {
                UserName = "cliente",
                Email = "cliente@purased.com",
                NombreCompleto = "Cliente Frecuente Pura Sed"
            };

            var resultadoCliente = await userManager.CreateAsync(cliente, "Cliente123");
            if (resultadoCliente.Succeeded)
            {
                await userManager.AddToRoleAsync(cliente, "Cliente");
                await userManager.AddToRoleAsync(cliente, "Usuario");
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(cliente, "Cliente"))
            {
                await userManager.AddToRoleAsync(cliente, "Cliente");
            }
            if (!await userManager.IsInRoleAsync(cliente, "Usuario"))
            {
                await userManager.AddToRoleAsync(cliente, "Usuario");
            }
        }
    }
}
