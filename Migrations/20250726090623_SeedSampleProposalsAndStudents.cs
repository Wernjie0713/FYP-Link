using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FYP_Link.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleProposalsAndStudents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
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
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s1-uid" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s2-uid" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s3-uid" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s4-uid" },
                    { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s5-uid" }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "ApplicationUserId", "ApprovalStatus", "MatricNumber", "Name", "SupervisorId" },
                values: new object[,]
                {
                    { 201, "s1-uid", "Approved", "A22EC1001", "Alice Tan", 101 },
                    { 202, "s2-uid", "Approved", "A22EC1002", "Bob Lee", 102 },
                    { 203, "s3-uid", "Approved", "A22EC1003", "Chloe Lim", 103 },
                    { 204, "s4-uid", "Approved", "A22EC1004", "Daniel Wong", 104 },
                    { 205, "s5-uid", "Approved", "A22EC1005", "Evelyn Ng", 105 }
                });

            migrationBuilder.InsertData(
                table: "Proposals",
                columns: new[] { "Id", "Abstract", "AcademicSession", "ProjectType", "Semester", "StudentId", "SupervisorId", "Title" },
                values: new object[,]
                {
                    { 1, "A system using facial recognition and AI to automate student attendance.", "2024/2025", 0, 1, 201, 101, "AI-Powered Attendance System" },
                    { 2, "A secure blockchain solution for storing and verifying academic records.", "2024/2025", 1, 1, 202, 102, "Blockchain for Academic Records" },
                    { 3, "An IoT-based greenhouse that monitors and controls climate for optimal plant growth.", "2024/2025", 0, 1, 203, 103, "IoT Smart Greenhouse" },
                    { 4, "A mobile application that helps students navigate campus buildings and facilities.", "2024/2025", 1, 1, 204, 104, "Mobile App for Campus Navigation" },
                    { 5, "A data analytics platform to analyze and predict student academic performance.", "2024/2025", 0, 1, 205, 105, "Data Analytics for Student Performance" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s1-uid" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s2-uid" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s3-uid" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s4-uid" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a18be9c0-aa65-4af8-bd17-00bd9344e576", "s5-uid" });

            migrationBuilder.DeleteData(
                table: "Proposals",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Proposals",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Proposals",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Proposals",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Proposals",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "s1-uid");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "s2-uid");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "s3-uid");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "s4-uid");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "s5-uid");
        }
    }
}
