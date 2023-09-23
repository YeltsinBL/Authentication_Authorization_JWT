using Microsoft.EntityFrameworkCore;
using LoginToken.Models;

using LoginToken.Service;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Permitir hacer peticiones externas
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:5019") //URL de la Web Local
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Referencia a la conexión de la base de datos
builder.Services.AddDbContext<SesionTokenContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("connectionSQL"));
});

// Registramos el Servicio para que se utilice en todo el proyecto
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

// Configurar JWT
var key = builder.Configuration.GetValue<string>("JwtSetting:secretKey");
var keyBytes = Encoding.ASCII.GetBytes(key);
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    // Deshabilitar el HTTPS
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // validar el usuario
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes), // credenciales del token
        ValidateIssuer = false, // quién solicita el token
        ValidateAudience = false, // desde dónde solicita el token
        ValidateLifetime = true, // tiempo de vida del token 
        ClockSkew = TimeSpan.Zero, // evitar desviación del tiempo de vida del token
    };
}) ; 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Permitir hacer peticiones externas
app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
