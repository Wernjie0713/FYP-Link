using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYP_Link.Data;
using FYP_Link.Models;

namespace FYP_Link.Controllers
{
    [ApiController]
    [Route("api/evaluations")]
    [Authorize(Roles = "Committee")]
    public class EvaluationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EvaluationsController(ApplicationDbContext context)
        {
            _context = context;
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

            return Ok(new { Message = "Evaluators assigned successfully." });
        }
    }
}