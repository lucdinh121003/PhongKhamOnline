using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhongKhamOnline.Migrations
{
    /// <inheritdoc />
    public partial class Remove_KhungGioBacSi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa khóa ngoại liên quan đến bảng KhungGioBacSis trong bảng BacSis
            migrationBuilder.DropForeignKey(
                name: "FK_BacSis_KhungGioBacSis_KhungGioBacSiId",
                table: "BacSis");

            // Xóa bảng KhungGioBacSis
            migrationBuilder.DropTable(
                name: "KhungGioBacSis");

            // Nếu bạn muốn xóa các chỉ mục (indexes) đã tạo
            migrationBuilder.DropIndex(
                name: "IX_BacSis_KhungGioBacSiId",
                table: "BacSis");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Khôi phục lại bảng KhungGioBacSis nếu cần (ví dụ trong trường hợp rollback)
            migrationBuilder.CreateTable(
                name: "KhungGioBacSis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GioLamViec = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhungGioBacSis", x => x.Id);
                });

            // Tạo lại khóa ngoại nếu cần
            migrationBuilder.AddForeignKey(
                name: "FK_BacSis_KhungGioBacSis_KhungGioBacSiId",
                table: "BacSis",
                column: "KhungGioBacSiId",
                principalTable: "KhungGioBacSis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
