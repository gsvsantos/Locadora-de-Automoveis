using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocadoraDeAutomoveis.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Add_Employee : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Employee",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                AdmissionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                Salary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Employee", x => x.Id);
                table.ForeignKey(
                    name: "FK_Employee_AspNetUsers_TenantId",
                    column: x => x.TenantId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Employee_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Employee_TenantId",
            table: "Employee",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_Employee_UserId",
            table: "Employee",
            column: "UserId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Employee_UserId_IsActive",
            table: "Employee",
            columns: new[] { "UserId", "IsActive" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Employee");
    }
}
