using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhongKhamOnline.Migrations
{
    /// <inheritdoc />
    public partial class Rename_IdKhungThoiGian_To_KhungThoiGianId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
           name: "IdKhungThoiGian",
           table: "LichLamViecs",
           newName: "KhungThoiGianId");

            // Đổi tên chỉ mục liên quan (nếu có)
            migrationBuilder.RenameIndex(
                name: "IX_LichLamViecs_IdKhungThoiGian",
                table: "LichLamViecs",
                newName: "IX_LichLamViecs_KhungThoiGianId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
           name: "KhungThoiGianId",
           table: "LichLamViecs",
           newName: "IdKhungThoiGian");

            // Đổi tên chỉ mục lại (nếu có)
            migrationBuilder.RenameIndex(
                name: "IX_LichLamViecs_KhungThoiGianId",
                table: "LichLamViecs",
                newName: "IX_LichLamViecs_IdKhungThoiGian");
        }
    }
}
