using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYP_Link.Data;
using FYP_Link.Models;

namespace FYP_Link.Controllers
{
    [ApiController]
    [Route("api/approvals/supervisors")]
    [Authorize(Roles = "Committee")]
    public class SupervisorApprovalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SupervisorApprovalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/approvals/supervisors/pending
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingSupervisorRequests()
        {
            var pending = await _context.Students
                .Where(s => s.ApprovalStatus == "Pending" && s.SupervisorId != null)
                .Include(s => s.Supervisor)
                .Select(s => new {
                    s.Id,
                    s.Name,
                    s.MatricNumber,
                    SupervisorName = s.Supervisor != null ? s.Supervisor.Name : null
                })
                .ToListAsync();
            return Ok(pending);
        }

        // POST: /api/approvals/supervisors/{studentId}/approve
        [HttpPost("{studentId}/approve")]
        public async Task<IActionResult> ApproveSupervisorRequest(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
                return NotFound();
            if (student.ApprovalStatus != "Pending")
                return BadRequest(new { Message = "Request is not pending." });
            student.ApprovalStatus = "Approved";
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Supervisor request approved." });
        }

        // POST: /api/approvals/supervisors/{studentId}/reject
        [HttpPost("{studentId}/reject")]
        public async Task<IActionResult> RejectSupervisorRequest(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
                return NotFound();
            if (student.ApprovalStatus != "Pending")
                return BadRequest(new { Message = "Request is not pending." });
            student.ApprovalStatus = "Rejected";
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Supervisor request rejected." });
        }
    }
} 