using EmployeeManagementSystem.Business.DTOs.Auth;
using FluentValidation;

namespace EmployeeManagementSystem.Business.Validators.Auth
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequestDto>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required");


        }
    }
}
