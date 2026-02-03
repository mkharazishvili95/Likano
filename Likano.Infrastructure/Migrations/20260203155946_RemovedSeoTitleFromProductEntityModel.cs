using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Likano.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedSeoTitleFromProductEntityModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeoTitle",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeoTitle",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
