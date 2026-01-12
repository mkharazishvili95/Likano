using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Likano.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedProducerCountryEntityModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProducerCountry",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "ProducerCountryId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProducerCountries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProducerCountries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProducerCountryId",
                table: "Products",
                column: "ProducerCountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProducerCountries_ProducerCountryId",
                table: "Products",
                column: "ProducerCountryId",
                principalTable: "ProducerCountries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProducerCountries_ProducerCountryId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProducerCountries");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProducerCountryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProducerCountryId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "ProducerCountry",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
