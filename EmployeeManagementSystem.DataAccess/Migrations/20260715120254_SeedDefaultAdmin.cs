using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "EmployeeCode", "FirstName", "IsActive", "LastName", "ManagerId", "MustChangePassword", "PasswordHash", "PhoneNumber", "RoleId", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2026, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@ems.com", "EMP0001", "System", true, "Admin", null, true, "$2a$12$MgMyDfo9MgheJK33AYLbRu/6r3CXeDu.Uy0bH471dt8lo8AOUR9yy", "9999999999", 1, new DateTime(2026, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
