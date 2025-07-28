using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYP_Link.Data;
using FYP_Link.Models;

namespace FYP_Link.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Student")]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var student = await _context.Students
                .Include(s => s.Supervisor)
                .Include(s => s.Proposal) // Eagerly load proposal
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student == null) return NotFound();

            return Ok(new
            {
                student.Name,
                student.MatricNumber,
                student.ApprovalStatus,
                student.SupervisorId,
                SupervisorName = student.Supervisor?.Name,
                // Add a flag for the frontend to easily check if a proposal exists
                HasProposal = student.Proposal != null
            });
        }

        [HttpGet("my-proposal")]
        public async Task<IActionResult> GetMyProposal()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var student = await _context.Students
                .Include(s => s.Proposal)
                    .ThenInclude(p => p.Supervisor)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student == null) return NotFound(new { Message = "Student profile not found" });

            if (student.Proposal == null) return NotFound(new { Message = "No proposal found" });

            return Ok(student.Proposal);
        }
        
        [HttpPut("my-profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateStudentProfileDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student == null) return NotFound();

            student.Name = dto.Name;
            student.MatricNumber = dto.MatricNumber;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Profile updated successfully" });
        }

        [HttpPost("select-supervisor")]
        public async Task<IActionResult> SelectSupervisor([FromBody] SelectSupervisorDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student == null) return NotFound(new { Message = "Student not found" });

            // Check if supervisor exists
            var supervisor = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.Id == dto.SupervisorId);

            if (supervisor == null)
                return BadRequest(new { Message = "Selected supervisor not found" });

            // Update student's supervisor and set approval status to pending
            student.SupervisorId = dto.SupervisorId;
            student.ApprovalStatus = "Pending";
            student.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Supervisor selection submitted successfully" });
        }
    }

    public class UpdateStudentProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string MatricNumber { get; set; } = string.Empty;
    }

    public class SelectSupervisorDto
    {
        public int SupervisorId { get; set; }
    }
}