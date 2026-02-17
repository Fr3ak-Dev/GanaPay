using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using GanaPay.Application.Interfaces;
using GanaPay.Application.Mappings;
using GanaPay.Application.Services;
using GanaPay.Application.Settings;
using GanaPay.Application.Validators;
using GanaPay.Core.Interfaces.Repositories;
using GanaPay.Infrastructure.Data;
using GanaPay.Infrastructure.Repositories;
using GanaPay.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ==================== CONFIGURAR SETTINGS ====================
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
// =============================================================

// ==================== CONFIGURAR DbContext ====================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// ==============================================================

// ==================== REGISTRAR REPOSITORIOS ====================
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
// ================================================================

// ==================== REGISTRAR SERVICIOS ====================
builder.Services.AddScoped<IAuthService, AuthService>();
// =============================================================

// ==================== CONFIGURAR AUTOMAPPER ====================
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
// ===============================================================

// ==================== CONFIGURAR FLUENTVALIDATION ====================
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
// =====================================================================

// ==================== CONFIGURAR CORS ====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "http://localhost:5057",
                "https://localhost:7057",
                "http://localhost:19006"
            )
            .AllowAnyMethod()       // GET, POST, PUT, DELETE, etc.
            .AllowAnyHeader()       // Authorization, Content-Type, etc.
            .AllowCredentials();    // Cookies, JWT en headers
    });
});
// =========================================================

// ==================== CONFIGURAR AUTENTICACIÓN JWT ====================
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // En desarrollo, permite HTTP
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero // Token expira exactamente cuando debe
    };
});

builder.Services.AddAuthorization();
// ======================================================================

// ==================== CONFIGURAR CONTROLLERS ====================
// Configuración para evitar/ignorar ciclos de referencia en JSON y omitir propiedades nulas
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
// ================================================================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "GanaPay API",
        Version = "v1",
        Description = "Sistema de Pagos Digitales"
    });

    // Configurar JWT en Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingresa: Bearer {tu token}\n\nEjemplo: Bearer eyJhbGci..."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ==================== SEED DATA ====================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await DataSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[SEED] ❌ Error durante el seed: {ex.Message}");
    }
}
// ===================================================

// ==================== CONFIGURAR PIPELINE ====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();  // ← PRIMERO: Verifica quién eres
app.UseAuthorization();   // ← SEGUNDO: Verifica qué puedes hacer

app.MapControllers();

app.Run();
// ============================================================