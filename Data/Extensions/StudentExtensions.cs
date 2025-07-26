using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FYP_Link.Data.Extensions
{
    public static class StudentExtensions
    {
        public static void HandleDuplicateMatricNumbers(this MigrationBuilder migrationBuilder)
        {
            // Create a temporary table to store row numbers for duplicate MatricNumber entries
            migrationBuilder.Sql(@"
                CREATE TEMP TABLE temp_duplicate_matric AS
                SELECT ""Id"", ""MatricNumber"",
                       ROW_NUMBER() OVER (PARTITION BY ""MatricNumber"" ORDER BY ""Id"") as rn
                FROM ""Students""
                WHERE ""MatricNumber"" IN (
                    SELECT ""MatricNumber"" 
                    FROM ""Students"" 
                    GROUP BY ""MatricNumber"" 
                    HAVING COUNT(*) > 1
                );

                UPDATE ""Students"" s
                SET ""MatricNumber"" = s.""MatricNumber"" || '_' || t.rn
                FROM temp_duplicate_matric t
                WHERE s.""Id"" = t.""Id"" AND t.rn > 1;

                DROP TABLE temp_duplicate_matric;
            ");
        }
    }
}