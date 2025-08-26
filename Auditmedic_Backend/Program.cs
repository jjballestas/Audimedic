
using Audimedic_Backend.Config;
using Audimedic_Backend.Data;
using Audimedic_Backend.Security;
using Audimedic_Backend.Services;
using Audimedic_Backend.Services.Historias;
using Audimedic_Backend.Services.Storage;
using Audimedic_Backend.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =======================
// 1. DbContext
// =======================
builder.Services.AddDbContext<AudimedicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =======================
// 2. Servicios propios
// =======================
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHistoriaQueryService, HistoriaQueryService>();
builder.Services.AddScoped<IProcesamientoHistoriaService, ProcesamientoHistoriaService>();
builder.Services.AddScoped<IHistoriaUploadService, HistoriaUploadService>();

builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.AddSingleton<IPrivateStorage, FileSystemPrivateStorage>();

// =======================
// 3. Authentication & Authorization (JWT)
// =======================
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretKey";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "AudimedicIssuer";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// =======================
// 4. Controllers + Swagger
// =======================
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("users-v1", new OpenApiInfo
    {
        Title = "Audimedic API (Users)",
        Version = "v1"
    });

    options.SwaggerDoc("admin-v1", new OpenApiInfo
    {
        Title = "Audimedic API (Admin)",
        Version = "v1"
    });

    // JWT en Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticación JWT usando el esquema Bearer. Ejemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// =======================
// 5. Middleware
// =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/users-v1/swagger.json", "Audimedic API (Users)");
        options.SwaggerEndpoint("/swagger/admin-v1/swagger.json", "Audimedic API (Admin)");
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  // ⚡ primero autenticación
app.UseAuthorization();

app.MapControllers();

app.Run();
