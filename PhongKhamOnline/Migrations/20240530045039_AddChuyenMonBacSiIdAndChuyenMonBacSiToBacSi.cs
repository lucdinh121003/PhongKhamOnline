using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhongKhamOnline.Migrations
{
    /// <inheritdoc />
    public partial class AddChuyenMonBacSiIdAndChuyenMonBacSiToBacSi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChuyenMon",
                table: "BacSis");

            migrationBuilder.AddColumn<int>(
                name: "ChuyenMonBacSiId",
                table: "BacSis",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChuyenMonBacSi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChuyenMon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuyenMonBacSi", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BacSis_ChuyenMonBacSiId",
                table: "BacSis",
                column: "ChuyenMonBacSiId");

            migrationBuilder.AddForeignKey(
                name: "FK_BacSis_ChuyenMonBacSi_ChuyenMonBacSiId",
                table: "BacSis",
                column: "ChuyenMonBacSiId",
                principalTable: "ChuyenMonBacSi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BacSis_ChuyenMonBacSi_ChuyenMonBacSiId",
                table: "BacSis");

            migrationBuilder.DropTable(
                name: "ChuyenMonBacSi");

            migrationBuilder.DropIndex(
                name: "IX_BacSis_ChuyenMonBacSiId",
                table: "BacSis");

            migrationBuilder.DropColumn(
                name: "ChuyenMonBacSiId",
                table: "BacSis");

            migrationBuilder.AddColumn<string>(
                name: "ChuyenMon",
                table: "BacSis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
