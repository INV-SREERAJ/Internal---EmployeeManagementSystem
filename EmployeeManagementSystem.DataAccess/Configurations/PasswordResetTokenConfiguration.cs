using EmployeeManagementSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Configurations
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.ToTable("PassWordResetTokens");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Token).IsRequired();
            builder.HasOne(p => p.Employee)
               .WithMany(u => u.PasswordResetTokens)
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
