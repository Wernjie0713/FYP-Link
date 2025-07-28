using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYP_Link.Data;
using FYP_Link.Models;
using Microsoft.AspNetCore.Identity;

namespace FYP_Link.Controllers
{
    [ApiController]
    [Route("api/evaluations")]
    [Authorize(Roles = "Committee")]
    public class EvaluationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EvaluationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /api/evaluations/available/{proposalId}
        [HttpGet("available/{proposalId}")]
        public async Task<IActionResult> GetAvailableEvaluators(int proposalId)
        {
            var proposal = await _context.Proposals.Include(p => p.Supervisor).FirstOrDefaultAsync(p => p.Id == proposalId);
            if (proposal == null)
                return NotFound();
            var projectType = proposal.ProjectType;
            var available = await _context.Lecturers
                .Where(l => l.Domain != null && 
                           l.Domain.ToUpper() == projectType.ToUpper() && 
                           l.Id != proposal.SupervisorId)
                .Select(l => new { l.Id, l.Name, l.Domain })
                .ToListAsync();
            return Ok(available);
        }

        public class AssignEvaluatorsDto
    {
        public int ProposalId { get; set; }
        public List<int> EvaluatorIds { get; set; } = new();
    }

    public class SubmitEvaluationDto
    {
        public string Result { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
    }

        // POST: /api/evaluations
        [HttpPost]
        public async Task<IActionResult> AssignEvaluators([FromBody] AssignEvaluatorsDto dto)
        {
            if (dto.EvaluatorIds == null || dto.EvaluatorIds.Count != 2)
                return BadRequest(new { Message = "Exactly two evaluators must be assigned." });
            var proposal = await _context.Proposals
                .Include(p => p.Evaluations)
                .FirstOrDefaultAsync(p => p.Id == dto.ProposalId);

            if (proposal == null)
                return NotFound();

            // Remove existing evaluations
            if (proposal.Evaluations.Any())
            {
                _context.Evaluations.RemoveRange(proposal.Evaluations);
                await _context.SaveChangesAsync();
            }

            // Create new evaluations
            var evaluations = dto.EvaluatorIds.Select(eid => new Evaluation
            {
                ProposalId = dto.ProposalId,
                EvaluatorId = eid
            }).ToList();

            _context.Evaluations.AddRange(evaluations);
            await _context.SaveChangesAsync();

            // Assign "Evaluator" role to the assigned lecturers
            foreach (var evaluatorId in dto.EvaluatorIds)
            {
                var lecturer = await _context.Lecturers.FindAsync(evaluatorId);
                if (lecturer != null)
                {
                    var user = await _userManager.FindByIdAsync(lecturer.ApplicationUserId);
                    if (user != null && !await _userManager.IsInRoleAsync(user, "Evaluator"))
                    {
                        await _userManager.AddToRoleAsync(user, "Evaluator");
                    }
                }
            }

            return Ok(new { Message = "Evaluators assigned successfully." });
        }

        // POST: /api/evaluations/{proposalId}/submit
        [HttpPost("{proposalId}/submit")]
        public async Task<IActionResult> SubmitEvaluation(int proposalId, [FromBody] SubmitEvaluationDto dto)
        {
            var proposal = await _context.Proposals
                .Include(p => p.Evaluations)
                .FirstOrDefaultAsync(p => p.Id == proposalId);

            if (proposal == null)
                return NotFound();

            if (string.IsNullOrEmpty(proposal.SupervisorComment))
                return StatusCode(403, new { Message = "You cannot evaluate this proposal until the supervisor has submitted their comments." });

            var evaluation = await _context.Evaluations
                .FirstOrDefaultAsync(e => e.ProposalId == proposalId && e.EvaluatorId == GetCurrentLecturerId());

            if (evaluation == null)
                return NotFound(new { Message = "You are not assigned to evaluate this proposal." });

            evaluation.Result = dto.Result;
            evaluation.Comments = dto.Comments;
            evaluation.CreatedAt = DateTime.UtcNow;
            evaluation.IsCurrent = true;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Evaluation submitted successfully." });
        }

        private int GetCurrentLecturerId()
        {
            var email = User.Identity?.Name;
            return _context.Lecturers
                .Where(l => l.Email == email)
                .Select(l => l.Id)
                .FirstOrDefault();
        }
    }
}