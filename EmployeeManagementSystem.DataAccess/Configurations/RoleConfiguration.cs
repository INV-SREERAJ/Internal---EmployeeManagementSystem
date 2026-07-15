using EmployeeManagementSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name)
               .IsRequired()
               .HasMaxLength(50);

            builder.Property(r => r.Description)
                   .HasMaxLength(200);

            builder.HasMany(r => r.Users)
                   .WithOne(u => u.Role)
                   .HasForeignKey(u => u.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Role
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "System Administrator"
                },
                new Role
                {
                    Id = 2,
                    Name = "Manager",
                    Description = "Reporting Manager"
                },
                new Role
                {
                    Id = 3,
                    Name = "Employee",
                    Description = "Employee"
                }
            );
        }
    }
}
