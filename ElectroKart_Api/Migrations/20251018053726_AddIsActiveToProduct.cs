using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroKart_Api.Migrations
{
    public partial class AddIsActiveToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add IsActive column
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: true);

            // Keep the Name alteration if needed
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove IsActive column if rolling back
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Products");

            // Revert Name column
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
