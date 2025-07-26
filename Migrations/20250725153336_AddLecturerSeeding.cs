using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FYP_Link.Migrations
{
    /// <inheritdoc />
    public partial class AddLecturerSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domain",
                table: "Lecturers");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e501", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e701", "lecturer1@example.com", true, false, null, "LECTURER1@EXAMPLE.COM", "LECTURER1@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e601", false, "lecturer1@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e502", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e702", "lecturer2@example.com", true, false, null, "LECTURER2@EXAMPLE.COM", "LECTURER2@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e602", false, "lecturer2@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e503", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e703", "lecturer3@example.com", true, false, null, "LECTURER3@EXAMPLE.COM", "LECTURER3@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e603", false, "lecturer3@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e504", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e704", "lecturer4@example.com", true, false, null, "LECTURER4@EXAMPLE.COM", "LECTURER4@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e604", false, "lecturer4@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e505", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e705", "lecturer5@example.com", true, false, null, "LECTURER5@EXAMPLE.COM", "LECTURER5@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e605", false, "lecturer5@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e506", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e706", "lecturer6@example.com", true, false, null, "LECTURER6@EXAMPLE.COM", "LECTURER6@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e606", false, "lecturer6@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e507", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e707", "lecturer7@example.com", true, false, null, "LECTURER7@EXAMPLE.COM", "LECTURER7@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e607", false, "lecturer7@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e508", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e708", "lecturer8@example.com", true, false, null, "LECTURER8@EXAMPLE.COM", "LECTURER8@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e608", false, "lecturer8@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e509", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e709", "lecturer9@example.com", true, false, null, "LECTURER9@EXAMPLE.COM", "LECTURER9@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e609", false, "lecturer9@example.com" },
                    { "b18be9c0-aa65-4af8-bd17-00bd9344e510", 0, "c18be9c0-aa65-4af8-bd17-00bd9344e710", "lecturer10@example.com", true, false, null, "LECTURER10@EXAMPLE.COM", "LECTURER10@EXAMPLE.COM", "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==", null, false, "b18be9c0-aa65-4af8-bd17-00bd9344e610", false, "lecturer10@example.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e501" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e502" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e503" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e504" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e505" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e506" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e507" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e508" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e509" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e510" }
                });

            migrationBuilder.InsertData(
                table: "Lecturers",
                columns: new[] { "Id", "ApplicationUserId", "CreatedAt", "CurrentPosition", "Department", "Name", "StaffId", "UpdatedAt" },
                values: new object[,]
                {
                    { 101, "b18be9c0-aa65-4af8-bd17-00bd9344e501", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SeniorLecturer", "Information Technology", "Dr. Lecturer 1", "STAFF00001", null },
                    { 102, "b18be9c0-aa65-4af8-bd17-00bd9344e502", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AssociateProfessor", "Software Engineering", "Dr. Lecturer 2", "STAFF00002", null },
                    { 103, "b18be9c0-aa65-4af8-bd17-00bd9344e503", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Professor", "Data Science", "Dr. Lecturer 3", "STAFF00003", null },
                    { 104, "b18be9c0-aa65-4af8-bd17-00bd9344e504", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lecturer", "Cybersecurity", "Dr. Lecturer 4", "STAFF00004", null },
                    { 105, "b18be9c0-aa65-4af8-bd17-00bd9344e505", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SeniorLecturer", "Computer Science", "Dr. Lecturer 5", "STAFF00005", null },
                    { 106, "b18be9c0-aa65-4af8-bd17-00bd9344e506", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AssociateProfessor", "Information Technology", "Dr. Lecturer 6", "STAFF00006", null },
                    { 107, "b18be9c0-aa65-4af8-bd17-00bd9344e507", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Professor", "Software Engineering", "Dr. Lecturer 7", "STAFF00007", null },
                    { 108, "b18be9c0-aa65-4af8-bd17-00bd9344e508", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lecturer", "Data Science", "Dr. Lecturer 8", "STAFF00008", null },
                    { 109, "b18be9c0-aa65-4af8-bd17-00bd9344e509", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SeniorLecturer", "Cybersecurity", "Dr. Lecturer 9", "STAFF00009", null },
                    { 110, "b18be9c0-aa65-4af8-bd17-00bd9344e510", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AssociateProfessor", "Computer Science", "Dr. Lecturer 10", "STAFF00010", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e501" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e502" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e503" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e504" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e505" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e506" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e507" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e508" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e509" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e577", "b18be9c0-aa65-4af8-bd17-00bd9344e510" });

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "Lecturers",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b18be9c0-aa65-4af8-bd17-00bd9344e501");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b18be9c0-aa65-4af8-bd17-00bd9344e502");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b18be9c0-aa65-4af8-bd17-00bd9344e503");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b18be9c0-aa65-4af8-bd17-00bd9344e504");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b18be9c0-aa65-4af8-bd17-00bd9344e505");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b18be9c0-aa65-4af8-bd17-00bd9344e506");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b18be9c0-aa65-4af8-bd17-00bd9344e507");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b18be9c0-aa65-4af8-bd17-00bd9344e508");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b18be9c0-aa65-4af8-bd17-00bd9344e509");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b18be9c0-aa65-4af8-bd17-00bd9344e510");

            migrationBuilder.AddColumn<int>(
                name: "Domain",
                table: "Lecturers",
                type: "integer",
                nullable: true);
        }
    }
}
