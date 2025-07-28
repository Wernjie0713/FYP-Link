using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYP_Link.Data;
using FYP_Link.Models;

namespace FYP_Link.Controllers
{
    [Authorize(Roles = "Supervisor")]
    [ApiController]
    [Route("api/[controller]")]
    public class SupervisorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public class SupervisorCommentDto
        {
            public string Comment { get; set; } = string.Empty;
        }

        public SupervisorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("my-students")]
        public async Task<IActionResult> GetMyStudents()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.ApplicationUserId  == user.Id);
            if (lecturer == null) return NotFound("Lecturer profile not found");

            var students = await _context.Students
                .Include(s => s.Proposal)
                .Where(s => s.SupervisorId == lecturer.Id && s.ApprovalStatus == "Approved")
                .Select(s => new
                {
                    s.Name,
                    s.MatricNumber,
                    Proposal = s.Proposal == null ? null : new
                    {
                        s.Proposal.Id,
                        s.Proposal.Title,
                        s.Proposal.AcademicSession,
                        s.Proposal.Semester
                    }
                })
                .ToListAsync();

            return Ok(students);
        }

        [HttpGet("proposals/{proposalId}")]
        public async Task<IActionResult> GetProposal(int proposalId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.ApplicationUserId  == user.Id);
            if (lecturer == null) return NotFound("Lecturer profile not found");

            var proposal = await _context.Proposals
                .Include(p => p.Student)
                .Include(p => p.Evaluations)
                    .ThenInclude(e => e.Evaluator)
                .FirstOrDefaultAsync(p => p.Id == proposalId && p.SupervisorId == lecturer.Id);

            if (proposal == null) return NotFound();

            return Ok(new
            {
                proposal.Id,
                proposal.Title,
                proposal.Abstract,
                proposal.ProjectType,
                proposal.AcademicSession,
                proposal.Semester,
                proposal.PdfFilePath,
                proposal.SupervisorComment,
                proposal.SupervisorCommentedAt,
                Student = new
                {
                    proposal.Student.Name,
                    proposal.Student.MatricNumber
                },
                Evaluations = proposal.Evaluations.Select(e => new
                {
                    e.Id,
                    e.Result,
                    e.Comments,
                    Evaluator = new
                    {
                        e.Evaluator.Name,
                        e.Evaluator.Email
                    },
                    e.CreatedAt
                })
            });
        }

        [HttpPost("proposals/{proposalId}/comment")]
        public async Task<IActionResult> AddComment(int proposalId, [FromBody] SupervisorCommentDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.ApplicationUserId == user.Id);
            if (lecturer == null) return NotFound("Lecturer profile not found");

            var proposal = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Id == proposalId && p.SupervisorId == lecturer.Id);

            if (proposal == null) return NotFound();

            proposal.SupervisorComment = dto.Comment;
            proposal.SupervisorCommentedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}