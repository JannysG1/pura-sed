using Microsoft.AspNetCore.Identity;
using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Data;

public static class IdentitySeeder
{
    private static readonly string[] Roles = { "Admin", "Usuario" };

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

        // 2. Credenciales del Admin desde la configuración (User Secrets o appsettings)
        var email = config["SeedAdmin:Email"] ?? "admin@cafeteria.com";
        var password = config["SeedAdmin:Password"] ?? "Admin123";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return; // sin credenciales, solo se crean los roles
        }

        // 3. Crear el usuario Admin si no existe
        var admin = await userManager.FindByEmailAsync(email);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin",
                Email = email,
                NombreCompleto = "Administrador"
            };

            var resultado = await userManager.CreateAsync(admin, password);
            if (!resultado.Succeeded)
            {
                var errores = string.Join(", ", resultado.Errors.Select(e => e.Description));
                throw new InvalidOperationException("No se pudo crear el usuario Admin: " + errores);
            }
        }

        // 4. Asignar el rol Admin si todavía no lo tiene
        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
