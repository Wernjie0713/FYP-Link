using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FYP_Link.Data;
using FYP_Link.Models;

namespace FYP_Link.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("admin-stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminStats()
        {
            try
            {
                // Calculate total students
                var totalStudents = await _context.Students.CountAsync();

                // Calculate total lecturers
                var totalLecturers = await _context.Lecturers.CountAsync();

                // Calculate total proposals
                var totalProposals = await _context.Proposals.CountAsync();

                // Calculate pending supervisor approvals (students with "Pending" status)
                var pendingSupervisorApprovals = await _context.Students
                    .Where(s => s.ApprovalStatus == "Pending")
                    .CountAsync();

                var stats = new
                {
                    totalStudents,
                    totalLecturers,
                    totalProposals,
                    pendingSupervisorApprovals
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving dashboard statistics", Error = ex.Message });
            }
        }

        [HttpGet("lecturer-summary")]
        [Authorize(Roles = "Supervisor,Committee,Evaluator")]
        public async Task<IActionResult> GetLecturerSummary()
        {
            try
            {
                // Get the current user's lecturer profile
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                var lecturer = await _context.Lecturers
                    .FirstOrDefaultAsync(l => l.ApplicationUserId == user.Id);
                if (lecturer == null) return NotFound("Lecturer profile not found");

                // Count actively supervised students
                var supervisedStudentsCount = await _context.Students
                    .Where(s => s.SupervisorId == lecturer.Id && s.ApprovalStatus == "Approved")
                    .CountAsync();

                // Count pending supervisor requests (for committee members)
                var pendingSupervisorRequestsCount = await _context.Students
                    .Where(s => s.ApprovalStatus == "Pending")
                    .CountAsync();

                // Count pending evaluation assignments
                var pendingEvaluationAssignmentsCount = await _context.Evaluations
                    .Where(e => e.EvaluatorId == lecturer.Id && e.IsCurrent && string.IsNullOrEmpty(e.Result))
                    .CountAsync();

                var summary = new
                {
                    supervisedStudentsCount,
                    pendingSupervisorRequestsCount,
                    pendingEvaluationAssignmentsCount
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving lecturer summary", Error = ex.Message });
            }
        }
    }
} 