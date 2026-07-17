using EmployeeManagementSystem.Business.DTOs.Admin;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.Entities;
using EmployeeManagementSystem.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IEmailService _emailService;

        public AdminService(IUserRepository userRepository, IPasswordService passwordService, IEmailService emailService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _emailService = emailService;
        }
        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            var emailExists = await _userRepository.EmailExistsAsync(request.Email);
            if (emailExists)
            {
                throw new Exception("Email Already exist");
            }

            var temporaryPassword = _passwordService.GenerateTemporaryPassword();

            var passwordHash = _passwordService.HashPassword(temporaryPassword);

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RoleId = request.RoleId,
                ManagerId = request.ManagerId,

                PasswordHash = passwordHash,

                IsActive = true,
                IsDeleted = false,
                MustChangePassword = true,

                TokenVersion = 1,

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _userRepository.AddUserAsync(user);
            user.EmployeeCode = $"EMP{(user.Id + 1120):D5}";
            await _userRepository.UpdateAsync(user);
            await _emailService.WelcomeEmailAsync(
                user.Email,
                $"{user.FirstName} {user.LastName}",
                temporaryPassword);
            return new CreateUserResponse
            {
                EmplooyeeCode = user.EmployeeCode,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Role = user.RoleId.ToString(),
                Message = "User created successfully."
            };
        }

    }
}
