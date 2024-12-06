using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhongKhamOnline.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSoLuongToiDa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoLuongToiDa",
                table: "LichLamViecs");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoLuongToiDa",
                table: "LichLamViecs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
