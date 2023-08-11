using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LapkaBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Soft_Delete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Shelters_ShelterId",
                table: "Animals");

            migrationBuilder.AlterColumn<Guid>(
                name: "ShelterId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "SoftDeleteAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ShelterId",
                table: "Users",
                column: "ShelterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Shelters_ShelterId",
                table: "Animals",
                column: "ShelterId",
                principalTable: "Shelters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Shelters_ShelterId",
                table: "Users",
                column: "ShelterId",
                principalTable: "Shelters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Shelters_ShelterId",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Shelters_ShelterId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ShelterId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SoftDeleteAt",
                table: "Users");

            migrationBuilder.AlterColumn<Guid>(
                name: "ShelterId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Shelters_ShelterId",
                table: "Animals",
                column: "ShelterId",
                principalTable: "Shelters",
                principalColumn: "Id");
        }
    }
}
