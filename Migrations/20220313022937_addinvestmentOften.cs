using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace trackitback.Migrations
{
    public partial class addinvestmentOften : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Often",
                table: "Investment",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Often",
                table: "Investment");
        }
    }
}
