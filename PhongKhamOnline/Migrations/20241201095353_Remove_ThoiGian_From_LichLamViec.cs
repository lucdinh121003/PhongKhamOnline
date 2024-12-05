using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhongKhamOnline.Migrations
{
    /// <inheritdoc />
    public partial class Remove_ThoiGian_From_LichLamViec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThoiGian",
                table: "LichLamViecs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Thêm lại cột ThoiGian nếu rollback migration
            migrationBuilder.AddColumn<string>(
                name: "ThoiGian",
                table: "LichLamViecs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
