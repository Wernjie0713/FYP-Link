using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYP_Link.Data;
using FYP_Link.Models;

namespace FYP_Link.Controllers
{
    [Authorize(Roles = "Evaluator")]
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluatorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public class EvaluationSubmissionDto
        {
            public string Result { get; set; } = string.Empty;
            public string Comments { get; set; } = string.Empty;
        }

        public EvaluatorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("my-assignments")]
        public async Task<IActionResult> GetMyAssignments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.ApplicationUserId == user.Id);
            if (lecturer == null) return NotFound("Evaluator profile not found");

            var assignments = await _context.Evaluations
                .Include(e => e.Proposal)
                    .ThenInclude(p => p.Student)
                .Include(e => e.Proposal)
                    .ThenInclude(p => p.Supervisor)
                .Where(e => e.EvaluatorId == lecturer.Id && e.IsCurrent)
                .Select(e => new
                {
                    EvaluationId = e.Id,
                    ProposalId = e.Proposal.Id,
                    Title = e.Proposal.Title,
                    StudentName = e.Proposal.Student.Name,
                    SupervisorName = e.Proposal.Supervisor.Name,
                    SupervisorComment = e.Proposal.SupervisorComment,
                    Result = e.Result,
                    Comments = e.Comments
                })
                .ToListAsync();

            return Ok(assignments);
        }

        [HttpGet("proposals/{proposalId}")]
        public async Task<IActionResult> GetProposalDetails(int proposalId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.ApplicationUserId == user.Id);
            if (lecturer == null) return NotFound("Evaluator profile not found");

            // Check if the evaluator is assigned to this proposal
            var evaluation = await _context.Evaluations
                .FirstOrDefaultAsync(e => e.ProposalId == proposalId && e.EvaluatorId == lecturer.Id && e.IsCurrent);
            if (evaluation == null) return NotFound("You are not assigned to evaluate this proposal");

            var proposal = await _context.Proposals
                .Include(p => p.Student)
                .Include(p => p.Supervisor)
                .Include(p => p.Evaluations)
                    .ThenInclude(e => e.Evaluator)
                .FirstOrDefaultAsync(p => p.Id == proposalId);

            if (proposal == null) return NotFound("Proposal not found");

            // Get all evaluations for this proposal (current and historical)
            var evaluations = await _context.Evaluations
                .Include(e => e.Evaluator)
                .Where(e => e.ProposalId == proposalId)
                .Select(e => new
                {
                    EvaluationId = e.Id,
                    EvaluatorName = e.Evaluator.Name,
                    EvaluatorId = e.EvaluatorId,
                    Result = e.Result,
                    Comments = e.Comments,
                    CreatedAt = e.CreatedAt,
                    IsCurrent = e.IsCurrent
                })
                .ToListAsync();

            return Ok(new
            {
                ProposalId = proposal.Id,
                Title = proposal.Title,
                Abstract = proposal.Abstract,
                ProjectType = proposal.ProjectType,
                AcademicSession = proposal.AcademicSession,
                Semester = proposal.Semester,
                PdfFilePath = proposal.PdfFilePath,
                SupervisorComment = proposal.SupervisorComment,
                SupervisorCommentedAt = proposal.SupervisorCommentedAt,
                Student = new
                {
                    Name = proposal.Student.Name,
                    MatricNumber = proposal.Student.MatricNumber
                },
                Supervisor = new
                {
                    Name = proposal.Supervisor.Name,
                    Email = proposal.Supervisor.Email
                },
                CurrentEvaluationId = evaluation.Id,
                Evaluations = evaluations
            });
        }

        [HttpPost("evaluations/{evaluationId}")]
        public async Task<IActionResult> SubmitEvaluation(int evaluationId, [FromBody] EvaluationSubmissionDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.ApplicationUserId == user.Id);
            if (lecturer == null) return NotFound("Evaluator profile not found");

            var evaluation = await _context.Evaluations
                .Include(e => e.Proposal)
                .FirstOrDefaultAsync(e => e.Id == evaluationId && e.EvaluatorId == lecturer.Id && e.IsCurrent);

            if (evaluation == null) return NotFound("Evaluation assignment not found");

            // Check if supervisor has submitted comments
            if (string.IsNullOrEmpty(evaluation.Proposal.SupervisorComment))
            {
                return StatusCode(403, new { Message = "You cannot evaluate this proposal until the supervisor has submitted their initial comments." });
            }

            evaluation.Result = dto.Result;
            evaluation.Comments = dto.Comments;
            evaluation.CreatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Evaluation submitted successfully." });
        }
    }
}