using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Likano.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class revertLastMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludedItems",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IncludedItems",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
