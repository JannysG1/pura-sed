using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using PuraSed.WebApi.Data;
using PuraSed.WebApi.Models;
using PuraSed.WebApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Permitir que el frontend Blazor acceda a la API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("PuraSedClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5068",
                "https://localhost:7020")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ---------- SERVICIOS ----------
builder.Services.AddControllers();

// Base de datos con Entity Framework Core (Paso 4)
builder.Services.AddDbContext<CafeteriaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CafeteriaDb")));

// Configuración de ASP.NET Core Identity (Guía 4 - Paso 3)
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<CafeteriaDbContext>();

// Repositorio Scoped para interactuar con DbContext por petición (Paso 8)
builder.Services.AddScoped<ProductoRepository>();

// Autenticación: "los usuarios se identifican con un JWT"
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // valida quién emitió el token
            ValidateAudience = true, // valida para quién es
            ValidateLifetime = true, // rechaza tokens vencidos
            ValidateIssuerSigningKey = true, // valida la firma
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Habilita las reglas de permisos ([Authorize], roles, políticas)
builder.Services.AddAuthorization();

// Swagger con soporte para JWT (Pasos 8 y 9)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega solo el token, sin la palabra Bearer"
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// ---------- PIPELINE ----------
var app = builder.Build();

// Sembrado de roles y Administrador inicial (Guía 4 - Paso 4)
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
}

app.UseHttpsRedirection();

// CORS debe ejecutarse antes de la autenticación/autorización
app.UseCors("PuraSedClient");

// Activar Swagger en el pipeline (solo en desarrollo) - Paso 10
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseAuthentication(); // 1.º: ¿quién eres? (lee el token)
app.UseAuthorization();  // 2.º: ¿tienes permiso?

app.MapControllers();

app.Run();
