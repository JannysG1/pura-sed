using Microsoft.AspNetCore.Identity;

namespace PuraSed.WebApi.Models;

public class ApplicationUser : IdentityUser
{
    public string NombreCompleto { get; set; } = string.Empty;
}
