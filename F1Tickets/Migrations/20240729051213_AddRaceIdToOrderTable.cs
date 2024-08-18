using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Tickets.Migrations
{
    /// <inheritdoc />
    public partial class AddRaceIdToOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RaceId",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RaceId",
                table: "Order");
        }
    }
}
