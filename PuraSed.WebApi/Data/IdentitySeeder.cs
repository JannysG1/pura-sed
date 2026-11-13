using Microsoft.AspNetCore.Identity;
using PuraSed.WebApi.Constants;
using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // 1. Crear todos los roles del ecosistema Pura Sed si no existen
        foreach (var rol in Roles.Todos)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole(rol));
            }
        }

        // Helper local para crear usuarios y asignar roles de forma segura e idempotente
        async Task AsegurarUsuarioAsync(string username, string email, string password, string nombreCompleto, params string[] rolesAsignar)
        {
            var user = await userManager.FindByNameAsync(username) ?? await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    NombreCompleto = nombreCompleto,
                    EmailConfirmed = true
                };

                var resultado = await userManager.CreateAsync(user, password);
                if (!resultado.Succeeded)
                {
                    var errores = string.Join(", ", resultado.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"No se pudo crear el usuario {username}: {errores}");
                }
            }

            foreach (var rol in rolesAsignar)
            {
                if (!await userManager.IsInRoleAsync(user, rol))
                {
                    await userManager.AddToRoleAsync(user, rol);
                }
            }
        }

        // 2. Administrador / Superadministrador
        var adminEmail = config["SeedAdmin:Email"] ?? "admin@purased.com";
        var adminPassword = config["SeedAdmin:Password"] ?? "Admin123";
        await AsegurarUsuarioAsync("admin", adminEmail, adminPassword, "Administrador Gerencial Pura Sed", Roles.Admin, Roles.SuperAdmin);

        // 3. Mesero (Toma de pedidos en mesas - HU-13)
        await AsegurarUsuarioAsync("mesero", "mesero@purased.com", "Mesero123", "Mesero Salón Principal", Roles.Mesero);

        // 4. Administrador de Cocina (KDS y Platos - HU-15)
        await AsegurarUsuarioAsync("cocina", "cocina@purased.com", "Cocina123", "Jefe de Cocina y Paila", Roles.AdminCocina);

        // 5. Administrador de Bar (KDD, Chichas y Cervezas - HU-15)
        await AsegurarUsuarioAsync("bar", "bar@purased.com", "Bar12345", "Bartender y Fermentos Pura Sed", Roles.AdminBar);

        // 6. Asistente (Reservas y asignación de mesas - HU-18)
        await AsegurarUsuarioAsync("asistente", "asistente@purased.com", "Asistente123", "Asistente de Reservas y Host", Roles.Asistente);

        // 7. Cliente (Consumidor final en tablet de mesa - HU-01, HU-13)
        await AsegurarUsuarioAsync("cliente", "cliente@purased.com", "Cliente123", "Cliente Frecuente Pura Sed", Roles.Cliente, Roles.Usuario);
    }
}
