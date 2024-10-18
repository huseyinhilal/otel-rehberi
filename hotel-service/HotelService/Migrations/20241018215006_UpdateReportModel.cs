using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelService.Migrations
{
    public partial class UpdateReportModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactCount",
                table: "Reports",
                newName: "PhoneNumberCount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumberCount",
                table: "Reports",
                newName: "ContactCount");
        }
    }
}
