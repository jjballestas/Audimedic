
using Audimedic_Backend.Config;
using Audimedic_Backend.Data;
using Audimedic_Backend.Services; 
using Audimedic_Backend.Services.Storage;
using Audimedic_Backend.Services.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; 
using Audimedic_Backend.Services.Historias;
 
using System.Text;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();

 
// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<IHistoriaQueryService, HistoriaQueryService>();
builder.Services.AddScoped<IProcesamientoHistoriaService, ProcesamientoHistoriaService>();

builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.AddSingleton<IPrivateStorage, FileSystemPrivateStorage>();
 
// NO registres StaticFiles para esa carpeta (no será pública).
// app.UseStaticFiles(); // <- esto solo sirve wwwroot, no tu Storage.RootPath
builder.Services.AddSingleton<IPrivateStorage, FileSystemPrivateStorage>(); builder.Services.AddSingleton<IPrivateStorage, FileSystemPrivateStorage>();
// Opciones de storage (ruta relativa, p.ej. "App_Data/Storage")
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));
// Storage privado (fuera de wwwroot)
builder.Services.AddSingleton<IPrivateStorage, FileSystemPrivateStorage>();

// Servicios de historias (query/processing si los usas) + upload
builder.Services.AddScoped<IHistoriaUploadService, HistoriaUploadService>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

builder.Services.AddDbContext<AudimedicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

app.UseAuthentication();
app.UseAuthorization();

app.Run();
