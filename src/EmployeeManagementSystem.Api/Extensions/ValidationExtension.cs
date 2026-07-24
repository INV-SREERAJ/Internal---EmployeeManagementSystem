using EmployeeManagementSystem.Business.Validators.Admin;
using EmployeeManagementSystem.Business.Validators.Auth;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagementSystem.Api.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();

        services.AddValidatorsFromAssemblyContaining<CreateEmployeeRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<RefreshTokenRequestValidator>();

        return services;
    }
}