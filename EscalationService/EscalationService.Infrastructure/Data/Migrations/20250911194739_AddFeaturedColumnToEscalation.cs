using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscalationService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFeaturedColumnToEscalation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Escalations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Escalations");
        }
    }
}
