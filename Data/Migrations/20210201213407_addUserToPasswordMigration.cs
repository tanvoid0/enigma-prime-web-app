using Microsoft.EntityFrameworkCore.Migrations;

namespace enigma_prime.Data.Migrations
{
    public partial class addUserToPasswordMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Password",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Password_UserId",
                table: "Password",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Password_AspNetUsers_UserId",
                table: "Password",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Password_AspNetUsers_UserId",
                table: "Password");

            migrationBuilder.DropIndex(
                name: "IX_Password_UserId",
                table: "Password");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Password");
        }
    }
}
