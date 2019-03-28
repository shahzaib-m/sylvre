using Microsoft.EntityFrameworkCore.Migrations;

namespace Sylvre.WebAPI.Migrations
{
    public partial class SylvreBlockSampleBlockProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSampleBlock",
                table: "SylvreBlocks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSampleBlock",
                table: "SylvreBlocks");
        }
    }
}
