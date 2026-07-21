using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FluentValidation;
//using EmployeeManagementSystem.DataAccess.Entities;
using EmployeeManagementSystem.Business.DTOs.Admin;

namespace EmployeeManagementSystem.Business.Validators.Admin
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First Name is required")
                .Length(2, 50)
                .WithMessage("Last name must be between 2 and 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last Name cannot be empty")
                .Length(1, 50)
                .WithMessage("Last name must be between 2 and 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email id is required.")
                .EmailAddress()
                .WithMessage("Enter a valid email address")
                .MaximumLength(100)
                .WithMessage("Email cannot exceed 100 characters.");

            RuleFor(x=> x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^[6-9]\d{9}$")
                .WithMessage("Please enter a valid 10-digit Indian mobile number.");

            RuleFor(x => x.RoleId)
                .GreaterThan(0)
                .LessThan(4)
                .WithMessage("Please select a valid role.");

            RuleFor(x => x.ManagerId)
                .GreaterThan(0)
                .When(x => x.ManagerId.HasValue)
                .WithMessage("Please select a valid manager.");
        }
    }
}
