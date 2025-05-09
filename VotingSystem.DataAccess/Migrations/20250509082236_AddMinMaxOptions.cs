using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMinMaxOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Maxvotes",
                table: "Polls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Minvotes",
                table: "Polls",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Maxvotes",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "Minvotes",
                table: "Polls");
        }
    }
}
