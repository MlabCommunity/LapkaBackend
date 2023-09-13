using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LapkaBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangingFieldNameInShelterVolunteering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TakingDogsOutDesctiption",
                table: "SheltersVolunteering",
                newName: "TakingDogsOutDescription");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TakingDogsOutDescription",
                table: "SheltersVolunteering",
                newName: "TakingDogsOutDesctiption");
        }
    }
}
