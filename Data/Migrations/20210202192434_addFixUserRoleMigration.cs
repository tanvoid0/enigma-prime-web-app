using Microsoft.EntityFrameworkCore.Migrations;

namespace enigma_prime.Data.Migrations
{
    public partial class addFixUserRoleMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "UserRole",
                newName: "Role");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "UserRole",
                newName: "UserName");
        }
    }
}
