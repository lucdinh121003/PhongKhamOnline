using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhongKhamOnline.Migrations
{
    /// <inheritdoc />
    public partial class RemoveKhungGioBacSiId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "KhungGioBacSiId",
                table: "BacSis");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.AddColumn<int>(
            name: "KhungGioBacSiId",
            table: "BacSis",
            type: "int",
            nullable: false,
            defaultValue: 0);
        }
    }
}
