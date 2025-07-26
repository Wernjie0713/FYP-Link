using Microsoft.EntityFrameworkCore.Migrations;
using FYP_Link.Data.Extensions;

#nullable disable

namespace FYP_Link.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToMatricNumberWithDuplicateHandling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Handle any duplicate MatricNumber entries before creating the unique index
            migrationBuilder.HandleDuplicateMatricNumbers();

            migrationBuilder.CreateIndex(
                name: "IX_Students_MatricNumber",
                table: "Students",
                column: "MatricNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_MatricNumber",
                table: "Students");
        }
    }
}
