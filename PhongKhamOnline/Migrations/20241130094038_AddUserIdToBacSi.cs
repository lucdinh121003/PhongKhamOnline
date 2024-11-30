using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhongKhamOnline.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToBacSi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        //    migrationBuilder.AddColumn<string>(
        //    name: "UserId",
        //    table: "BacSis",
        //    type: "nvarchar(450)", // Đảm bảo kiểu dữ liệu đúng
        //    nullable: true,
        //    defaultValue: null // Bạn không cần đặt defaultValue là "", chỉ cần là null nếu cột có thể chứa null
        //);

        //    migrationBuilder.AddForeignKey(
        //       name: "FK_BacSis_AspNetUsers_UserId",
        //       table: "BacSis",
        //       column: "UserId",
        //       principalTable: "AspNetUsers",
        //       principalColumn: "Id",
        //       onDelete: ReferentialAction.SetNull); // Thử SetNull thay vì Cascade
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
