using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LapkaBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSheltersVolunteeringTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SheltersVolunteering",
                columns: table => new
                {
                    ShelterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDonationActive = table.Column<bool>(type: "bit", nullable: false),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonationDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDailyHelpActive = table.Column<bool>(type: "bit", nullable: false),
                    dailyHelpDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTakingDogsOutActive = table.Column<bool>(type: "bit", nullable: false),
                    TakingDogsOutDesctiption = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheltersVolunteering", x => x.ShelterId);
                    table.ForeignKey(
                        name: "FK_SheltersVolunteering_Shelters_ShelterId",
                        column: x => x.ShelterId,
                        principalTable: "Shelters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SheltersVolunteering");
        }
    }
}
