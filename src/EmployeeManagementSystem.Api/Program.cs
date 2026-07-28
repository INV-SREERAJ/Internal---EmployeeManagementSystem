using EmployeeManagementSystem.Api.Extensions;
using EmployeeManagementSystem.Api.Middleware;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddValidationServices();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

app.UseMiddleware<Exceptions>();

app.UseAuthentication();
app.UseMiddleware<MustChangePasswordMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();