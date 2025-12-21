using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocadoraDeAutomoveis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_PreferredLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreferredLanguage",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreferredLanguage",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                table: "AspNetUsers");
        }
    }
}
