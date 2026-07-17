using EmployeeManagementSystem.Business.Configuration;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.Business.Services;
using EmployeeManagementSystem.DataAccess.Context;
using EmployeeManagementSystem.DataAccess.Interfaces;
using EmployeeManagementSystem.DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IJwtService, JwtService>();
//builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRefreshTokenGraceCache,
                              RefreshTokenGraceCache>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration
                    .GetSection("JwtSettings")
                    .Get<JwtSettings>()!;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthentication();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();  
