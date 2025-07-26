using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP_Link.Migrations
{
    /// <inheritdoc />
    public partial class AddSupervisorApprovalFieldsToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "Students",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "Students",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_SupervisorId",
                table: "Students",
                column: "SupervisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Lecturers_SupervisorId",
                table: "Students",
                column: "SupervisorId",
                principalTable: "Lecturers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Lecturers_SupervisorId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_SupervisorId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "Students");
        }
    }
}
