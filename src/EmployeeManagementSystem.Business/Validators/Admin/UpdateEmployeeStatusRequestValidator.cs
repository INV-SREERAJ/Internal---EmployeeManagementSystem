using EmployeeManagementSystem.Business.DTOs.Admin;
using EmployeeManagementSystem.DataAccess.Entities.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.Validators.Admin
{
    public class UpdateEmployeeStatusRequestValidator : AbstractValidator<UpdateEmployeeStatusRequest>
    {
        public UpdateEmployeeStatusRequestValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid employee status.");
        }
    }
}
