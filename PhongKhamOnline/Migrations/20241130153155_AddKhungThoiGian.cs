using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhongKhamOnline.Migrations
{
    /// <inheritdoc />
    public partial class AddKhungThoiGian : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KhungThoiGians",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhungThoiGian", x => x.Id);
                });

            // Seed data
            migrationBuilder.InsertData(
                table: "KhungThoiGians",
                columns: new[] { "Id", "Time" },
                values: new object[,]
                {
                    { 1, "7:00-7:30" },
                    { 2, "7:30-8:00" },
                    { 3, "8:00-8:30" },
                    { 4, "8:30-9:00" },
                    { 5, "9:00-9:30" },
                    { 6, "9:30-10:00" },
                    { 7, "10:00-10:30" },
                    { 8, "10:30-11:00" },
                    { 9, "13:30-14:00" },
                    { 10, "14:00-14:30" },
                    { 11, "14:30-15:00" },
                    { 12, "15:00-15:30" },
                    { 13, "15:30-16:00" },
                    { 14, "16:00-16:30" },
                    { 15, "16:30-17:00" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KhungThoiGians");
        }
    }
}
