using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LapkaBackend.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddedRolePropertyToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Shelters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Shelters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
