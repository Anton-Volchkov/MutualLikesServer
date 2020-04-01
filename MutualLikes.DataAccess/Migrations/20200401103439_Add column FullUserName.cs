using Microsoft.EntityFrameworkCore.Migrations;

namespace MutualLikes.DataAccess.Migrations
{
    public partial class AddcolumnFullUserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullUserName",
                table: "UserDatas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullUserName",
                table: "UserDatas");
        }
    }
}
