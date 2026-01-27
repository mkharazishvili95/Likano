using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Likano.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedIncludedComponentsIntoProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IncludedComponents",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludedComponents",
                table: "Products");
        }
    }
}
