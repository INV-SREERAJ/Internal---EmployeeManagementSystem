using EmployeeManagementSystem.Api.Middleware;
using EmployeeManagementSystem.Business.Configuration;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.Business.Services;
using EmployeeManagementSystem.Business.Validators;
using EmployeeManagementSystem.Business.Validators.Admin;
using EmployeeManagementSystem.Business.Validators.Auth;
using EmployeeManagementSystem.DataAccess.Context;
using EmployeeManagementSystem.DataAccess.Interfaces;
using EmployeeManagementSystem.DataAccess.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RefreshTokenRequestValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Employee Management System API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token only. Example: eyJhbGciOiJIUzI1NiIs..."
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
//builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
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
    
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
app.UseMiddleware<EmployeeManagementSystem.Api.Middleware.Exceptions>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();  
