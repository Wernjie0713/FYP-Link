using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FYP_Link.Data;
using FYP_Link.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYP_Link.Data.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedDatabaseAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created and migrations are applied
            await context.Database.MigrateAsync();

            // 1. Seed Roles
            await SeedRolesAsync(roleManager);

            // 2. Create Demo Accounts
            await SeedDemoAccountsAsync(userManager, context);

            // 3. Create Additional Dummy Data
            await SeedAdditionalDataAsync(userManager, context);

            // Save all changes
            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // Define the roles
            string[] roleNames = { "Admin", "Student", "Supervisor", "Committee", "Evaluator" };

            // Create roles if they don't exist
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task SeedDemoAccountsAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            // 1. Create admin account
            var adminUser = await CreateUserIfNotExistsAsync(
                userManager,
                "admin@example.com",
                "Password123!",
                new[] { "Admin" }
            );

            // 2. Create academician account
            var academicianUser = await CreateUserIfNotExistsAsync(
                userManager,
                "academician@example.com",
                "Password123!",
                new[] { "Supervisor", "Committee", "Evaluator" }
            );

            // Create lecturer profile for academician
            if (academicianUser != null && !context.Lecturers.Any(l => l.ApplicationUserId == academicianUser.Id))
            {
                var lecturer = new Lecturer
                {
                    Name = "Dr. Jane Smith",
                    StaffId = "STAFF12345",
                    Department = "Computer Science",
                    CurrentPosition = "Associate Professor",
                    ApplicationUserId = academicianUser.Id,
                    Email = "academician@example.com",
                    Domain = "Research"
                };

                context.Lecturers.Add(lecturer);
                await context.SaveChangesAsync();
            }

            // 3. Create student account
            var studentUser = await CreateUserIfNotExistsAsync(
                userManager,
                "student@example.com",
                "Password123!",
                new[] { "Student" }
            );

            // Create student profile
            if (studentUser != null && !context.Students.Any(s => s.ApplicationUserId == studentUser.Id))
            {
                // Get the lecturer ID for the academician
                var lecturer = await context.Lecturers
                    .FirstOrDefaultAsync(l => l.ApplicationUserId == academicianUser.Id);

                var student = new Student
                {
                    Name = "John Doe",
                    MatricNumber = "A22EC9999",
                    ApplicationUserId = studentUser.Id,
                    SupervisorId = lecturer?.Id,
                    ApprovalStatus = "Approved"
                };

                context.Students.Add(student);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedAdditionalDataAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            // 1. Create 9 more lecturers
            var lecturers = await SeedAdditionalLecturersAsync(userManager, context);

            // 2. Create 9 more students
            var students = await SeedAdditionalStudentsAsync(userManager, context, lecturers);

            // 3. Create proposals for students
            await SeedProposalsAsync(context, students, lecturers);
        }

        private static async Task<List<Lecturer>> SeedAdditionalLecturersAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            var lecturers = new List<Lecturer>();
            var departments = new[] { "Computer Science", "Information Technology", "Software Engineering", "Data Science", "Cybersecurity" };
            var positions = new[] { "Lecturer", "Senior Lecturer", "Associate Professor", "Professor" };
            var domains = new[] { "Research", "Development" };
            var firstNames = new[] { "Emma", "Noah", "Olivia", "Liam", "Ava", "William", "Sophia", "James", "Isabella" };
            var lastNames = new[] { "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Taylor" };

            for (int i = 0; i < 9; i++)
            {
                var email = $"lecturer{i + 1}@example.com";
                var name = $"Dr. {firstNames[i]} {lastNames[i]}";
                var staffId = $"STAFF{10001 + i}";
                var department = departments[i % departments.Length];
                var position = positions[i % positions.Length];
                var domain = domains[i % domains.Length];

                // Create user account
                var user = await CreateUserIfNotExistsAsync(
                    userManager,
                    email,
                    "Password123!",
                    new[] { "Supervisor", "Evaluator" }
                );

                if (user != null && !context.Lecturers.Any(l => l.Email == email))
                {
                    var lecturer = new Lecturer
                    {
                        Name = name,
                        StaffId = staffId,
                        Department = department,
                        CurrentPosition = position,
                        ApplicationUserId = user.Id,
                        Email = email,
                        Domain = domain
                    };

                    context.Lecturers.Add(lecturer);
                    await context.SaveChangesAsync();
                    lecturers.Add(lecturer);
                }
                else
                {
                    // If the lecturer already exists, add it to our list
                    var existingLecturer = await context.Lecturers.FirstOrDefaultAsync(l => l.Email == email);
                    if (existingLecturer != null)
                    {
                        lecturers.Add(existingLecturer);
                    }
                }
            }

            return lecturers;
        }

        private static async Task<List<Student>> SeedAdditionalStudentsAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context, List<Lecturer> lecturers)
        {
            var students = new List<Student>();
            var firstNames = new[] { "Michael", "Sarah", "David", "Emily", "Daniel", "Jessica", "Matthew", "Ashley", "Andrew" };
            var lastNames = new[] { "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez" };
            var random = new Random();

            for (int i = 0; i < 9; i++)
            {
                var email = $"student{i + 1}@example.com";
                var name = $"{firstNames[i]} {lastNames[i]}";
                var matricNumber = $"A22EC{1001 + i}";

                // Create user account
                var user = await CreateUserIfNotExistsAsync(
                    userManager,
                    email,
                    "Password123!",
                    new[] { "Student" }
                );

                if (user != null && !context.Students.Any(s => s.MatricNumber == matricNumber))
                {
                    // Randomly assign a supervisor
                    var supervisorIndex = random.Next(lecturers.Count);
                    var supervisor = lecturers[supervisorIndex];

                    var student = new Student
                    {
                        Name = name,
                        MatricNumber = matricNumber,
                        ApplicationUserId = user.Id,
                        SupervisorId = supervisor.Id,
                        ApprovalStatus = "Approved",
                        UpdatedAt = DateTime.UtcNow
                    };

                    context.Students.Add(student);
                    await context.SaveChangesAsync();
                    students.Add(student);
                }
                else
                {
                    // If the student already exists, add it to our list
                    var existingStudent = await context.Students.FirstOrDefaultAsync(s => s.MatricNumber == matricNumber);
                    if (existingStudent != null)
                    {
                        students.Add(existingStudent);
                    }
                }
            }

            return students;
        }

        private static async Task SeedProposalsAsync(ApplicationDbContext context, List<Student> students, List<Lecturer> lecturers)
        {
            var proposalTitles = new[]
            {
                "AI-Powered Attendance System",
                "Blockchain for Academic Records",
                "IoT Smart Greenhouse",
                "Mobile App for Campus Navigation",
                "Data Analytics for Student Performance",
                "Virtual Reality Learning Environment"
            };

            var proposalAbstracts = new[]
            {
                "A system using facial recognition and AI to automate student attendance tracking in classrooms. The system will use deep learning algorithms to identify students and record their attendance in real-time.",
                "A secure blockchain solution for storing and verifying academic records. This system will provide tamper-proof storage of academic credentials and allow for easy verification by employers and other institutions.",
                "An IoT-based greenhouse that monitors and controls climate conditions for optimal plant growth. The system will use sensors to track temperature, humidity, and soil moisture, and automatically adjust conditions as needed.",
                "A mobile application that helps students navigate campus buildings and facilities. The app will provide indoor navigation, class schedules, and information about campus resources.",
                "A data analytics platform to analyze and predict student academic performance. The system will use machine learning to identify at-risk students and provide early intervention strategies.",
                "A virtual reality environment designed to enhance learning experiences across various subjects. The platform will allow students to interact with 3D models and simulations to better understand complex concepts."
            };

            var random = new Random();

            // Create 5-6 proposals
            for (int i = 0; i < 6; i++)
            {
                // Skip if we've reached the end of our student list
                if (i >= students.Count) break;

                var student = students[i];
                var supervisor = await context.Lecturers.FindAsync(student.SupervisorId);

                if (supervisor == null) continue;

                // Determine project type based on supervisor's domain
                var projectType = supervisor.Domain ?? (random.Next(2) == 0 ? "Research" : "Development");

                // Check if student already has a proposal
                var existingProposal = await context.Proposals.FirstOrDefaultAsync(p => p.StudentId == student.Id);
                if (existingProposal != null) continue;

                var proposal = new Proposal
                {
                    Title = proposalTitles[i],
                    Abstract = proposalAbstracts[i],
                    ProjectType = projectType,
                    AcademicSession = "2024/2025",
                    Semester = 1,
                    StudentId = student.Id,
                    SupervisorId = supervisor.Id
                };

                // Add supervisor comments to some proposals
                if (i % 2 == 0)
                {
                    proposal.SupervisorComment = $"This is a promising proposal. The student has shown good understanding of the {projectType} methodology. I recommend approval with minor revisions.";
                    proposal.SupervisorCommentedAt = DateTime.UtcNow.AddDays(-random.Next(1, 10));
                }

                context.Proposals.Add(proposal);
                await context.SaveChangesAsync();

                // Add evaluations for proposals with supervisor comments
                if (proposal.SupervisorComment != null)
                {
                    await SeedEvaluationsAsync(context, proposal, supervisor, lecturers);
                }
            }
        }

        private static async Task SeedEvaluationsAsync(ApplicationDbContext context, Proposal proposal, Lecturer supervisor, List<Lecturer> allLecturers)
        {
            var random = new Random();
            var evaluationResults = new[]
            {
                "Accepted",
                "Accepted with Minor Revisions",
                "Accepted with Major Revisions",
                "Rejected"
            };

            var evaluationComments = new[]
            {
                "The proposal is well-structured with clear objectives and methodology.",
                "Good proposal but needs more detail on implementation strategy.",
                "The literature review is comprehensive but the methodology needs refinement.",
                "The scope is too ambitious for a single semester project.",
                "Excellent proposal with innovative approach to the problem.",
                "The proposal lacks sufficient detail on data collection methods."
            };

            // Find eligible evaluators (domain matches project type and not the supervisor)
            var eligibleEvaluators = allLecturers
                .Where(l => l.Domain == proposal.ProjectType && l.Id != supervisor.Id)
                .ToList();

            // If we don't have enough eligible evaluators, add some with different domains
            if (eligibleEvaluators.Count < 2)
            {
                var additionalEvaluators = allLecturers
                    .Where(l => l.Id != supervisor.Id && !eligibleEvaluators.Contains(l))
                    .Take(2 - eligibleEvaluators.Count)
                    .ToList();

                eligibleEvaluators.AddRange(additionalEvaluators);
            }

            // Ensure we have at least 2 evaluators
            if (eligibleEvaluators.Count >= 2)
            {
                // Randomly select 2 evaluators
                var shuffledEvaluators = eligibleEvaluators.OrderBy(x => random.Next()).Take(2).ToList();

                // Create evaluations
                foreach (var evaluator in shuffledEvaluators)
                {
                    var evaluation = new Evaluation
                    {
                        ProposalId = proposal.Id,
                        EvaluatorId = evaluator.Id,
                        IsCurrent = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 5))
                    };

                    // Add result and comments to 1-2 evaluations
                    if (random.Next(3) > 0) // 2/3 chance of having results
                    {
                        evaluation.Result = evaluationResults[random.Next(evaluationResults.Length)];
                        evaluation.Comments = evaluationComments[random.Next(evaluationComments.Length)];
                    }

                    context.Evaluations.Add(evaluation);
                }

                await context.SaveChangesAsync();
            }
        }

        private static async Task<ApplicationUser?> CreateUserIfNotExistsAsync(UserManager<ApplicationUser> userManager, string email, string password, string[] roles)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    // Assign roles
                    foreach (var role in roles)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
                else
                {
                    // If user creation failed, return null
                    return null;
                }
            }

            return user;
        }
    }
}