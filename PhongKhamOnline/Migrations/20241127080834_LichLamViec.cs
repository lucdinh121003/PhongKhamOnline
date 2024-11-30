using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhongKhamOnline.Migrations
{
    /// <inheritdoc />
    public partial class LichLamViec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LichLamViecs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BacSiId = table.Column<int>(type: "int", nullable: false),
                    NgayLamViec = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGian = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoLuongToiDa = table.Column<int>(type: "int", nullable: false),
                    
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichLamViecs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichLamViecs_BacSis_BacSiId",
                        column: x => x.BacSiId,
                        principalTable: "BacSis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LichLamViecs_BacSiId",
                table: "LichLamViecs",
                column: "BacSiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LichLamViecs");
        }
    }
}
