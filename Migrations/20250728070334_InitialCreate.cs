using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FYP_Link.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicPrograms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicPrograms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lecturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StaffId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CurrentPosition = table.Column<string>(type: "text", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Domain = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecturers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lecturers_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommitteeMemberships",
                columns: table => new
                {
                    LecturerId = table.Column<int>(type: "integer", nullable: false),
                    AcademicProgramId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeMemberships", x => x.LecturerId);
                    table.ForeignKey(
                        name: "FK_CommitteeMemberships_AcademicPrograms_AcademicProgramId",
                        column: x => x.AcademicProgramId,
                        principalTable: "AcademicPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommitteeMemberships_Lecturers_LecturerId",
                        column: x => x.LecturerId,
                        principalTable: "Lecturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MatricNumber = table.Column<string>(type: "text", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    SupervisorId = table.Column<int>(type: "integer", nullable: true),
                    ApprovalStatus = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Students_Lecturers_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Lecturers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Proposals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Abstract = table.Column<string>(type: "text", nullable: false),
                    ProjectType = table.Column<string>(type: "text", nullable: false),
                    AcademicSession = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Semester = table.Column<int>(type: "integer", nullable: false),
                    PdfFilePath = table.Column<string>(type: "text", nullable: true),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    SupervisorId = table.Column<int>(type: "integer", nullable: false),
                    SupervisorComment = table.Column<string>(type: "text", nullable: true),
                    SupervisorCommentedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proposals_Lecturers_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Lecturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Proposals_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evaluations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Result = table.Column<string>(type: "text", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: false),
                    ProposalId = table.Column<int>(type: "integer", nullable: false),
                    EvaluatorId = table.Column<int>(type: "integer", nullable: false),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e575", null, "Admin", "ADMIN" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e576", null, "Student", "STUDENT" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", null, "Supervisor", "SUPERVISOR" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e578", null, "Evaluator", "EVALUATOR" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e579", null, "Committee", "COMMITTEE" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e57a", 0, "a18be9c0-aa65-4af8-bd17-00bd9344e57c", "admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "a18be9c0-aa65-4af8-bd17-00bd9344e57b", false, "admin@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e501", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e701", "lecturer1@example.com", true, false, null, "LECTURER1@EXAMPLE.COM", "LECTURER1@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e601", false, "lecturer1@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e502", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e702", "lecturer2@example.com", true, false, null, "LECTURER2@EXAMPLE.COM", "LECTURER2@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e602", false, "lecturer2@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e503", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e703", "lecturer3@example.com", true, false, null, "LECTURER3@EXAMPLE.COM", "LECTURER3@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e603", false, "lecturer3@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e504", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e704", "lecturer4@example.com", true, false, null, "LECTURER4@EXAMPLE.COM", "LECTURER4@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e604", false, "lecturer4@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e505", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e705", "lecturer5@example.com", true, false, null, "LECTURER5@EXAMPLE.COM", "LECTURER5@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e605", false, "lecturer5@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e506", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e706", "lecturer6@example.com", true, false, null, "LECTURER6@EXAMPLE.COM", "LECTURER6@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e606", false, "lecturer6@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e507", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e707", "lecturer7@example.com", true, false, null, "LECTURER7@EXAMPLE.COM", "LECTURER7@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e607", false, "lecturer7@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e508", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e708", "lecturer8@example.com", true, false, null, "LECTURER8@EXAMPLE.COM", "LECTURER8@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e608", false, "lecturer8@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e509", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e709", "lecturer9@example.com", true, false, null, "LECTURER9@EXAMPLE.COM", "LECTURER9@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e609", false, "lecturer9@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e510", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e710", "lecturer10@example.com", true, false, null, "LECTURER10@EXAMPLE.COM", "LECTURER10@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e610", false, "lecturer10@example.com" },
                    { "s1-uid", 0, "s1-uid-con", "student1@example.com", true, false, null, "STUDENT1@EXAMPLE.COM", "STUDENT1@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "s1-uid-sec", false, "student1@example.com" },
                    { "s2-uid", 0, "s2-uid-con", "student2@example.com", true, false, null, "STUDENT2@EXAMPLE.COM", "STUDENT2@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "s2-uid-sec", false, "student2@example.com" },
                    { "s3-uid", 0, "s3-uid-con", "student3@example.com", true, false, null, "STUDENT3@EXAMPLE.COM", "STUDENT3@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "s3-uid-sec", false, "student3@example.com" },
                    { "s4-uid", 0, "s4-uid-con", "student4@example.com", true, false, null, "STUDENT4@EXAMPLE.COM", "STUDENT4@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "s4-uid-sec", false, "student4@example.com" },
                    { "s5-uid", 0, "s5-uid-con", "student5@example.com", true, false, null, "STUDENT5@EXAMPLE.COM", "STUDENT5@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "s5-uid-sec", false, "student5@example.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e575", "a18be9c0-aa65-4af8-bd17-00bd9344e57a" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e501" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e502" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e503" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e504" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e505" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e506" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e507" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e508" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e509" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e510" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s1-uid" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s2-uid" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s3-uid" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s4-uid" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s5-uid" }
                });

            migrationBuilder.InsertData(
                table: "Lecturers",
                columns: new[] { "Id", "ApplicationUserId", "CreatedAt", "CurrentPosition", "Department", "Domain", "Email", "Name", "StaffId", "UpdatedAt" },
                values: new object[,]
                {
                    { 101, "b18be9c0-aa65-4af8-bd17-00bd9344e501", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SeniorLecturer", "Information Technology", null, "", "Dr. Lecturer 1", "STAFF00001", null },
                    { 102, "b18be9c0-aa65-4af8-bd17-00bd9344e502", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AssociateProfessor", "Software Engineering", null, "", "Dr. Lecturer 2", "STAFF00002", null },
                    { 103, "b18be9c0-aa65-4af8-bd17-00bd9344e503", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Professor", "Data Science", null, "", "Dr. Lecturer 3", "STAFF00003", null },
                    { 104, "b18be9c0-aa65-4af8-bd17-00bd9344e504", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lecturer", "Cybersecurity", null, "", "Dr. Lecturer 4", "STAFF00004", null },
                    { 105, "b18be9c0-aa65-4af8-bd17-00bd9344e505", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SeniorLecturer", "Computer Science", null, "", "Dr. Lecturer 5", "STAFF00005", null },
                    { 106, "b18be9c0-aa65-4af8-bd17-00bd9344e506", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AssociateProfessor", "Information Technology", null, "", "Dr. Lecturer 6", "STAFF00006", null },
                    { 107, "b18be9c0-aa65-4af8-bd17-00bd9344e507", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Professor", "Software Engineering", null, "", "Dr. Lecturer 7", "STAFF00007", null },
                    { 108, "b18be9c0-aa65-4af8-bd17-00bd9344e508", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lecturer", "Data Science", null, "", "Dr. Lecturer 8", "STAFF00008", null },
                    { 109, "b18be9c0-aa65-4af8-bd17-00bd9344e509", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SeniorLecturer", "Cybersecurity", null, "", "Dr. Lecturer 9", "STAFF00009", null },
                    { 110, "b18be9c0-aa65-4af8-bd17-00bd9344e510", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AssociateProfessor", "Computer Science", null, "", "Dr. Lecturer 10", "STAFF00010", null }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "ApplicationUserId", "ApprovalStatus", "MatricNumber", "Name", "SupervisorId", "UpdatedAt" },
                values: new object[,]
                {
                    { 201, "s1-uid", "Approved", "A22EC1001", "Alice Tan", 101, null },
                    { 202, "s2-uid", "Approved", "A22EC1002", "Bob Lee", 102, null },
                    { 203, "s3-uid", "Approved", "A22EC1003", "Chloe Lim", 103, null },
                    { 204, "s4-uid", "Approved", "A22EC1004", "Daniel Wong", 104, null },
                    { 205, "s5-uid", "Approved", "A22EC1005", "Evelyn Ng", 105, null }
                });

            migrationBuilder.InsertData(
                table: "Proposals",
                columns: new[] { "Id", "Abstract", "AcademicSession", "PdfFilePath", "ProjectType", "Semester", "StudentId", "SupervisorComment", "SupervisorCommentedAt", "SupervisorId", "Title" },
                values: new object[,]
                {
                    { 1, "A system using facial recognition and AI to automate student attendance.", "2024/2025", null, "Research", 1, 201, null, null, 101, "AI-Powered Attendance System" },
                    { 2, "A secure blockchain solution for storing and verifying academic records.", "2024/2025", null, "Development", 1, 202, null, null, 102, "Blockchain for Academic Records" },
                    { 3, "An IoT-based greenhouse that monitors and controls climate for optimal plant growth.", "2024/2025", null, "Research", 1, 203, null, null, 103, "IoT Smart Greenhouse" },
                    { 4, "A mobile application that helps students navigate campus buildings and facilities.", "2024/2025", null, "Development", 1, 204, null, null, 104, "Mobile App for Campus Navigation" },
                    { 5, "A data analytics platform to analyze and predict student academic performance.", "2024/2025", null, "Research", 1, 205, null, null, 105, "Data Analytics for Student Performance" }
                });

            migrationBuilder.InsertData(
                table: "Evaluations",
                columns: new[] { "Id", "Comments", "CreatedAt", "EvaluatorId", "IsCurrent", "ProposalId", "Result" },
                values: new object[,]
                {
                    { 1, "Good idea, but needs more detail on implementation.", new DateTime(2025, 7, 28, 7, 3, 33, 574, DateTimeKind.Utc).AddTicks(846), 106, true, 1, "Accepted with Conditions" },
                    { 2, "Solid methodology and clear outcomes.", new DateTime(2025, 7, 28, 7, 3, 33, 574, DateTimeKind.Utc).AddTicks(2059), 110, true, 1, "Accepted" },
                    { 3, "The scope is too broad for a single semester.", new DateTime(2025, 7, 28, 7, 3, 33, 574, DateTimeKind.Utc).AddTicks(2082), 107, true, 2, "Rejected" },
                    { 4, "Excellent proposal with clear objectives.", new DateTime(2025, 7, 28, 7, 3, 33, 574, DateTimeKind.Utc).AddTicks(2101), 108, true, 3, "Accepted" },
                    { 5, "Solid methodology and clear outcomes.", new DateTime(2025, 7, 28, 7, 3, 33, 574, DateTimeKind.Utc).AddTicks(2119), 108, true, 3, "Accepted" },
                    { 6, "Please clarify the data sources.", new DateTime(2025, 7, 28, 7, 3, 33, 574, DateTimeKind.Utc).AddTicks(2139), 109, true, 4, "Accepted with Minor Revisions" },
                    { 7, "Well-structured and feasible project.", new DateTime(2025, 7, 28, 7, 3, 33, 574, DateTimeKind.Utc).AddTicks(2157), 110, true, 5, "Accepted" },
                    { 8, "Solid methodology and clear outcomes.", new DateTime(2025, 7, 28, 7, 3, 33, 574, DateTimeKind.Utc).AddTicks(2176), 106, true, 5, "Accepted" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeMemberships_AcademicProgramId",
                table: "CommitteeMemberships",
                column: "AcademicProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_EvaluatorId",
                table: "Evaluations",
                column: "EvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_ProposalId",
                table: "Evaluations",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_Lecturers_ApplicationUserId",
                table: "Lecturers",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_StudentId",
                table: "Proposals",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_SupervisorId",
                table: "Proposals",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ApplicationUserId",
                table: "Students",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_MatricNumber",
                table: "Students",
                column: "MatricNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_SupervisorId",
                table: "Students",
                column: "SupervisorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CommitteeMemberships");

            migrationBuilder.DropTable(
                name: "Evaluations");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AcademicPrograms");

            migrationBuilder.DropTable(
                name: "Proposals");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Lecturers");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
