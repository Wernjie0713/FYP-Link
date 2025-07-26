using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP_Link.Migrations
{
    /// <inheritdoc />
    public partial class UniqueLecturerCommitteeAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CommitteeMemberships",
                table: "CommitteeMemberships");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommitteeMemberships",
                table: "CommitteeMemberships",
                column: "LecturerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CommitteeMemberships",
                table: "CommitteeMemberships");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommitteeMemberships",
                table: "CommitteeMemberships",
                columns: new[] { "LecturerId", "AcademicProgramId" });
        }
    }
}
