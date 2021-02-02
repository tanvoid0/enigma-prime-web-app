using Microsoft.EntityFrameworkCore.Migrations;

namespace enigma_prime.Data.Migrations
{
    public partial class addUserRoleMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoleName",
                table: "UserRole",
                newName: "UserName");

            migrationBuilder.AddColumn<bool>(
                name: "IsSelected",
                table: "UserRole",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSelected",
                table: "UserRole");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "UserRole",
                newName: "RoleName");
        }
    }
}
