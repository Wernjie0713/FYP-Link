// In Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FYP_Link.Models;

namespace FYP_Link.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<AcademicProgram> AcademicPrograms { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<CommitteeMembership> CommitteeMemberships { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Configure one-to-one relationship between ApplicationUser and Student
            builder.Entity<Student>()
                .HasOne(s => s.ApplicationUser)
                .WithOne()
                .HasForeignKey<Student>(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-one relationship between Student and Proposal
            builder.Entity<Student>()
                .HasOne(s => s.Proposal)
                .WithOne(p => p.Student)
                .HasForeignKey<Proposal>(p => p.StudentId);

            // Add unique constraint to MatricNumber
            builder.Entity<Student>()
                .HasIndex(s => s.MatricNumber)
                .IsUnique();

            base.OnModelCreating(builder);

            // Configure Lecturer entity
            builder.Entity<Lecturer>()
                .HasOne(l => l.ApplicationUser)
                .WithOne()
                .HasForeignKey<Lecturer>(l => l.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Lecturer>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // No need for conversions since both CurrentPosition and Domain are now strings

            // Configure AcademicProgram entity
            builder.Entity<AcademicProgram>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Configure CommitteeMembership entity with LecturerId as primary key
            builder.Entity<CommitteeMembership>()
                .HasKey(cm => cm.LecturerId);

            builder.Entity<CommitteeMembership>()
                .HasOne(cm => cm.Lecturer)
                .WithOne()
                .HasForeignKey<CommitteeMembership>(cm => cm.LecturerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CommitteeMembership>()
                .HasOne(cm => cm.AcademicProgram)
                .WithMany()
                .HasForeignKey(cm => cm.AcademicProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            // Existing role and admin user seeding
            var adminRoleId = "a18be9c0-aa65-4af8-bd17-00bd9344e575";
            var studentRoleId = "a18be9c0-aa65-4af8-bd17-00bd9344e576";
            var supervisorRoleId = "a18be9c0-aa65-4af8-bd17-00bd9344e577";
            var evaluatorRoleId = "a18be9c0-aa65-4af8-bd17-00bd9344e578";
            var committeeRoleId = "a18be9c0-aa65-4af8-bd17-00bd9344e579";

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = studentRoleId, Name = "Student", NormalizedName = "STUDENT" },
                new IdentityRole { Id = supervisorRoleId, Name = "Supervisor", NormalizedName = "SUPERVISOR" },
                new IdentityRole { Id = evaluatorRoleId, Name = "Evaluator", NormalizedName = "EVALUATOR" },
                new IdentityRole { Id = committeeRoleId, Name = "Committee", NormalizedName = "COMMITTEE" }
            );

            // Seed a default Admin User
            var adminId = "a18be9c0-aa65-4af8-bd17-00bd9344e57a";
            var adminSecurityStamp = "a18be9c0-aa65-4af8-bd17-00bd9344e57b";
            var adminPasswordHash = "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw==";

            var adminUser = new ApplicationUser
            {
                Id = adminId,
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = adminSecurityStamp,
                PasswordHash = adminPasswordHash,
                ConcurrencyStamp = "a18be9c0-aa65-4af8-bd17-00bd9344e57c"
            };

            builder.Entity<ApplicationUser>().HasData(adminUser);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminId
            });

            // Seed 10 sample lecturers with their user accounts
            var departments = new[] { "Computer Science", "Information Technology", "Software Engineering", "Data Science", "Cybersecurity" };
            var positions = new[] { "Lecturer", "SeniorLecturer", "AssociateProfessor", "Professor" };
            var passwordHash = "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw=="; // Password123!

            for (int i = 1; i <= 10; i++)
            {
                var userId = $"b18be9c0-aa65-4af8-bd17-00bd9344e5{i:D2}";
                var securityStamp = $"b18be9c0-aa65-4af8-bd17-00bd9344e6{i:D2}";
                var email = $"lecturer{i}@example.com";

                // Create lecturer profile
                builder.Entity<Lecturer>().HasData(new Lecturer
                {
                    Id = i + 100, // Start from 101 to avoid conflicts
                    Name = $"Dr. Lecturer {i}",
                    StaffId = $"STAFF{i:D5}",
                    Department = departments[i % departments.Length],
                    CurrentPosition = positions[i % positions.Length],
                    ApplicationUserId = userId,
                    CreatedAt = DateTime.Parse("2024-01-01T00:00:00Z").ToUniversalTime()
                });

                // Create user account
                builder.Entity<ApplicationUser>().HasData(new ApplicationUser
                {
                    Id = userId,
                    UserName = email,
                    NormalizedUserName = email.ToUpper(),
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = securityStamp,
                    PasswordHash = passwordHash,
                    ConcurrencyStamp = $"c18be9c0-aa65-4af8-bd17-00bd9344e7{i:D2}"
                });

                // Assign Supervisor role
                builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
                {
                    RoleId = supervisorRoleId,
                    UserId = userId
                });
            }

            // === SEED 5 STUDENTS WITH APPROVED SUPERVISORS ===
            var studentPasswordHash = "AQAAAAIAAYagAAAAEEZXQJGDm/Zq+YxDFhLhQbYIhYYBRvX2KwGGGpZZOQzGHgXYoQB0J7LxVJXZBQUJZw=="; // Password123!
            var students = new List<(string userId, string email, string name, string matric, int studentId, int supervisorId)>
            {
                ("s1-uid", "student1@example.com", "Alice Tan", "A22EC1001", 201, 101),
                ("s2-uid", "student2@example.com", "Bob Lee", "A22EC1002", 202, 102),
                ("s3-uid", "student3@example.com", "Chloe Lim", "A22EC1003", 203, 103),
                ("s4-uid", "student4@example.com", "Daniel Wong", "A22EC1004", 204, 104),
                ("s5-uid", "student5@example.com", "Evelyn Ng", "A22EC1005", 205, 105),
            };
            foreach (var (userId, email, name, matric, studentId, supervisorId) in students)
            {
                builder.Entity<ApplicationUser>().HasData(new ApplicationUser
                {
                    Id = userId,
                    UserName = email,
                    NormalizedUserName = email.ToUpper(),
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = userId + "-sec",
                    PasswordHash = studentPasswordHash,
                    ConcurrencyStamp = userId + "-con"
                });
                builder.Entity<IdentityUserRole<string>>().HasData(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
                {
                    RoleId = studentRoleId,
                    UserId = userId
                });
                builder.Entity<Student>().HasData(new Student
                {
                    Id = studentId,
                    Name = name,
                    MatricNumber = matric,
                    ApplicationUserId = userId,
                    SupervisorId = supervisorId,
                    ApprovalStatus = "Approved"
                });
            }

            // === SEED 5 PROPOSALS ===
            var proposalTitles = new[]
            {
                "AI-Powered Attendance System",
                "Blockchain for Academic Records",
                "IoT Smart Greenhouse",
                "Mobile App for Campus Navigation",
                "Data Analytics for Student Performance"
            };
            var proposalAbstracts = new[]
            {
                "A system using facial recognition and AI to automate student attendance.",
                "A secure blockchain solution for storing and verifying academic records.",
                "An IoT-based greenhouse that monitors and controls climate for optimal plant growth.",
                "A mobile application that helps students navigate campus buildings and facilities.",
                "A data analytics platform to analyze and predict student academic performance."
            };
            for (int i = 0; i < 5; i++)
            {
                builder.Entity<Proposal>().HasData(new Proposal
                {
                    Id = i + 1,
                    Title = proposalTitles[i],
                    Abstract = proposalAbstracts[i],
                    ProjectType = i % 2 == 0 ? "Research" : "Development", // Use string
                    AcademicSession = "2024/2025",
                    Semester = 1,
                    StudentId = 201 + i,
                    SupervisorId = 101 + i
                });
            }

            // === SEED EVALUATIONS FOR PROPOSALS ===
            var evaluationResults = new[]
            {
                ("Accepted with Conditions", "Good idea, but needs more detail on implementation."),
                ("Rejected", "The scope is too broad for a single semester."),
                ("Accepted", "Excellent proposal with clear objectives."),
                ("Accepted with Minor Revisions", "Please clarify the data sources."),
                ("Accepted", "Well-structured and feasible project.")
            };
            // Use lecturers 106, 107, 108, 109, 110 as evaluators (not supervisors 101-105)
            int evalId = 1;
            for (int i = 0; i < 5; i++)
            {
                // First evaluation for each proposal
                builder.Entity<Evaluation>().HasData(new Evaluation
                {
                    Id = evalId++,
                    Result = evaluationResults[i].Item1,
                    Comments = evaluationResults[i].Item2,
                    ProposalId = i + 1,
                    EvaluatorId = 106 + (i % 5)
                });
                // Second evaluation for some proposals
                if (i % 2 == 0) // For proposals 1, 3, 5
                {
                    builder.Entity<Evaluation>().HasData(new Evaluation
                    {
                        Id = evalId++,
                        Result = "Accepted",
                        Comments = "Solid methodology and clear outcomes.",
                        ProposalId = i + 1,
                        EvaluatorId = 110 - (i % 5) // Use a different lecturer
                    });
                }
            }
        }
    }
}