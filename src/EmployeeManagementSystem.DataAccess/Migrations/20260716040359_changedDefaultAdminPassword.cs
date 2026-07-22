using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class changedDefaultAdminPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$j6rZXXE38.Thjp6aP1gqN.l5vhHT3Ym32VRq/ns4Edi3HQloOEAKO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$MgMyDfo9MgheJK33AYLbRu/6r3CXeDu.Uy0bH471dt8lo8AOUR9yy");
        }
    }
}
