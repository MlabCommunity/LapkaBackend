using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LapkaBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingColumnRegistrationTokenIntoUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegistrationToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationToken",
                table: "Users");
        }
    }
}
