using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LapkaBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBankAccountPropertiesInShelterAndChangeFloatOnDoubleInCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Shelters",
                type: "float",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Shelters",
                type: "float",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                table: "Shelters",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccount",
                table: "Shelters");

            migrationBuilder.AlterColumn<float>(
                name: "Longitude",
                table: "Shelters",
                type: "real",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<float>(
                name: "Latitude",
                table: "Shelters",
                type: "real",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldMaxLength: 255);
        }
    }
}
