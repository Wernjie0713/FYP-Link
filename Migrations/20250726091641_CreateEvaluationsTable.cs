using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FYP_Link.Migrations
{
    /// <inheritdoc />
    public partial class CreateEvaluationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Evaluations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Result = table.Column<string>(type: "text", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: false),
                    ProposalId = table.Column<int>(type: "integer", nullable: false),
                    EvaluatorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluations_Lecturers_EvaluatorId",
                        column: x => x.EvaluatorId,
                        principalTable: "Lecturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evaluations_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Evaluations",
                columns: new[] { "Id", "Comments", "EvaluatorId", "ProposalId", "Result" },
                values: new object[,]
                {
                    { 1, "Good idea, but needs more detail on implementation.", 106, 1, "Accepted with Conditions" },
                    { 2, "Solid methodology and clear outcomes.", 110, 1, "Accepted" },
                    { 3, "The scope is too broad for a single semester.", 107, 2, "Rejected" },
                    { 4, "Excellent proposal with clear objectives.", 108, 3, "Accepted" },
                    { 5, "Solid methodology and clear outcomes.", 108, 3, "Accepted" },
                    { 6, "Please clarify the data sources.", 109, 4, "Accepted with Minor Revisions" },
                    { 7, "Well-structured and feasible project.", 110, 5, "Accepted" },
                    { 8, "Solid methodology and clear outcomes.", 106, 5, "Accepted" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_EvaluatorId",
                table: "Evaluations",
                column: "EvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_ProposalId",
                table: "Evaluations",
                column: "ProposalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evaluations");
        }
    }
}
