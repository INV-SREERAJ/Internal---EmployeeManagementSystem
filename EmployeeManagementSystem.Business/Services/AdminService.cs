using EmployeeManagementSystem.Business.DTOs.Admin;
using EmployeeManagementSystem.Business.GlobalExceptionHandler;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.common;
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
        private readonly IRoleRepository _roleRepository;
        private readonly IAdminRepository _adminRepository;
        public AdminService(IUserRepository userRepository, IPasswordService passwordService, IEmailService emailService,
            IRoleRepository roleRepository, IAdminRepository adminRepository)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _emailService = emailService;
            _roleRepository = roleRepository;
            _adminRepository = adminRepository;
        }
        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            var emailExists = await _userRepository.EmailExistsAsync(request.Email);
            if (emailExists)
            {
                throw new ConflictException("Email already exists!!");
            }


            //verification if the role id actually present.
            if (!await _roleRepository.RoleExistsById(request.RoleId)) 
            {
                throw new NotFoundException("Role not found.");
            }

            //checking with manager id.
            if (request.ManagerId.HasValue)
            {
                var manager = await _userRepository.GetByIdAsync(request.ManagerId.Value);

                if (manager == null)
                    throw new NotFoundException("Manager not found.");
                if (!manager.IsActive || manager.IsDeleted)
                {
                    throw new ConflictException("Selected manager is inactive.");
                }
                if (manager.Role.Name != "Manager" &&
                    manager.Role.Name != "Admin")
                {
                    throw new ConflictException("Selected user cannot be assigned as manager.");
                }
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

        public async Task<PagedResponse<UserListDto>> GetUsersAsync(UserQueryParameters parameters)
        {
            var (users, totalCount) =
                await _adminRepository.GetUsersAsync(parameters);

            var userDtos = users.Select(user => new UserListDto
            {
                Id = user.Id,
                EmployeeCode = user.EmployeeCode,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.Name,
                ManagerName = user.Manager == null
                    ? null
                    : $"{user.Manager.FirstName} {user.Manager.LastName}",
                IsActive = user.IsActive
            });

            return new PagedResponse<UserListDto>
            {
                Data = userDtos,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize)
            };
        }
    }
}
