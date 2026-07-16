using EmployeeManagementSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.HasIndex(u => u.Email)
                   .IsUnique();

            builder.Property(u => u.EmployeeCode)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(u => u.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(u => u.PhoneNumber)
                   .HasMaxLength(20);

            builder.Property(u => u.PasswordHash)
                   .IsRequired();

            builder.Property(u => u.TokenVersion)
                   .HasDefaultValue(0);

            builder.Property(u => u.IsActive)
                   .HasDefaultValue(true);

            builder.Property(u => u.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(u => u.MustChangePassword)
                   .HasDefaultValue(true);

            builder.HasOne(u => u.Role)
                   .WithMany(r => r.Users)
                   .HasForeignKey(u => u.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.Manager)
                   .WithMany(u => u.Employees)
                   .HasForeignKey(u => u.ManagerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
            new User
            {
                Id = 1,
                EmployeeCode = "EMP0001",
                FirstName = "System",
                LastName = "Admin",
                Email = "admin@ems.com",
                PhoneNumber = "9999999999",
                PasswordHash = "$2a$12$j6rZXXE38.Thjp6aP1gqN.l5vhHT3Ym32VRq/ns4Edi3HQloOEAKO",
                RoleId = 1,              // Admin
                ManagerId = null,
                TokenVersion = 0,
                MustChangePassword = true,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = new DateTime(2026, 7, 15),
                UpdatedAt = new DateTime(2026, 7, 15)
            });
            }
    }
}
