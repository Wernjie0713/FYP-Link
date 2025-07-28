using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FYP_Link.Data;
using FYP_Link.Models;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace FYP_Link.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProposalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        
        public ProposalsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: /api/proposals?session=2024/2025&semester=1
        [HttpGet]
        [Authorize(Roles = "Committee")]
        public async Task<IActionResult> GetProposals([FromQuery] string? session, [FromQuery] int? semester)
        {
            var query = _context.Proposals
                .Include(p => p.Student)
                .Include(p => p.Supervisor)
                .Include(p => p.Evaluations)
                    .ThenInclude(e => e.Evaluator)
                .AsQueryable();

            if (!string.IsNullOrEmpty(session))
                query = query.Where(p => p.AcademicSession == session);
            if (semester.HasValue)
                query = query.Where(p => p.Semester == semester.Value);

            var proposals = await query
                .Select(p => new {
                    p.Id,
                    p.Title,
                    p.Abstract,
                    ProjectType = p.ProjectType.ToString(),
                    p.AcademicSession,
                    p.Semester,
                    StudentName = p.Student.Name,
                    SupervisorName = p.Supervisor.Name,
                    Evaluations = p.Evaluations
                    .Where(e => e.IsCurrent)
                    .Select(e => new {
                        e.Id,
                        e.Result,
                        e.Comments,
                        EvaluatorId = e.EvaluatorId,
                        EvaluatorName = e.Evaluator.Name
                    }).ToList()
                })
                .ToListAsync();

            return Ok(proposals);
        }
        
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitProposal([FromForm] ProposalSubmissionDto dto)
        {
            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            
            // Get student profile
            var student = await _context.Students
                .Include(s => s.Supervisor)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);
                
            if (student == null) return NotFound(new { Message = "Student profile not found" });
            
            // Check if student has an approved supervisor
            if (student.ApprovalStatus != "Approved" || student.SupervisorId == null)
            {
                return StatusCode(403, new { Message = "You must have an approved supervisor before submitting a proposal" });
            }
            
            // Check if student already has a proposal
            var existingProposal = await _context.Proposals
                .AnyAsync(p => p.StudentId == student.Id);
                
            if (existingProposal)
            {
                return BadRequest(new { Message = "You have already submitted a proposal" });
            }
            
            // Create new proposal
            var proposal = new Proposal
            {
                Title = dto.Title,
                Abstract = dto.Abstract ?? "",
                ProjectType = dto.ProjectType,
                AcademicSession = dto.AcademicSession,
                Semester = dto.Semester,
                StudentId = student.Id,
                SupervisorId = student.SupervisorId.Value
            };
            
            // Handle file upload if provided
            if (dto.PdfFile != null && dto.PdfFile.Length > 0)
            {
                // Create directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "proposals");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                
                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}_{dto.PdfFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.PdfFile.CopyToAsync(fileStream);
                }
                
                // Store file path
                proposal.PdfFilePath = $"/proposals/{uniqueFileName}";
            }
            
            // Save proposal
            _context.Proposals.Add(proposal);
            await _context.SaveChangesAsync();
            
            return Ok(new { Message = "Proposal submitted successfully", ProposalId = proposal.Id });
        }
        
        [HttpGet("my-proposal")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyProposal()
        {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);
        if (student == null) return NotFound(new { Message = "Student profile not found" });
        
        var proposal = await _context.Proposals
            .Include(p => p.Supervisor)
            .Include(p => p.Evaluations)
                .ThenInclude(e => e.Evaluator)
            .Where(p => p.StudentId == student.Id)
            .Select(p => new {
                p.Id,
                p.Title,
                p.Abstract,
                p.ProjectType,
                p.AcademicSession,
                p.Semester,
                p.PdfFilePath,
                p.SupervisorComment,
                p.SupervisorCommentedAt,
                SupervisorName = p.Supervisor.Name,
                Evaluations = p.Evaluations.Select(e => new {
                    e.Result,
                    e.Comments,
                    e.IsCurrent,
                    EvaluatorName = e.Evaluator.Name
                }).ToList()
            })
            .FirstOrDefaultAsync();
            
        if (proposal == null) return NotFound(new { Message = "No proposal found" });
        
        return Ok(proposal);
    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> ResubmitProposal(int id, [FromForm] ProposalSubmissionDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);
        if (student == null) return NotFound(new { Message = "Student profile not found" });
        
        var proposal = await _context.Proposals
            .Include(p => p.Evaluations)
            .FirstOrDefaultAsync(p => p.Id == id && p.StudentId == student.Id);
        if (proposal == null) return NotFound(new { Message = "Proposal not found" });
        
        // Update proposal properties
        proposal.Title = dto.Title;
        proposal.Abstract = dto.Abstract ?? "";
        proposal.ProjectType = dto.ProjectType;
        proposal.AcademicSession = dto.AcademicSession;
        proposal.Semester = dto.Semester;
        
        // Handle new file upload if provided
        if (dto.PdfFile != null && dto.PdfFile.Length > 0)
        {
            // Delete old file if exists
            if (!string.IsNullOrEmpty(proposal.PdfFilePath))
            {
                var oldFilePath = Path.Combine(_environment.WebRootPath, proposal.PdfFilePath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            
            // Save new file
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "proposals");
            var uniqueFileName = $"{Guid.NewGuid()}_{dto.PdfFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await dto.PdfFile.CopyToAsync(fileStream);
            }
            
            proposal.PdfFilePath = $"/proposals/{uniqueFileName}";
        }
        
        // Get list of current evaluator IDs
        var evaluatorIds = proposal.Evaluations.Select(e => e.EvaluatorId).Distinct().ToList();
        
        // Mark existing evaluations as historical
        foreach (var evaluation in proposal.Evaluations)
        {
            evaluation.IsCurrent = false;
        }
        
        // Create new evaluations for each original evaluator
        foreach (var evaluatorId in evaluatorIds)
        {
            var newEvaluation = new Evaluation
            {
                ProposalId = proposal.Id,
                EvaluatorId = evaluatorId,
                IsCurrent = true,
                Result = string.Empty,
                Comments = string.Empty
            };
            _context.Evaluations.Add(newEvaluation);
        }
        
        await _context.SaveChangesAsync();
        
        return Ok(new { Message = "Proposal resubmitted successfully" });
        }
    }
    
    public class ProposalSubmissionDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string? Abstract { get; set; }
        
        [Required]
        public string ProjectType { get; set; } = string.Empty;
        
        [Required]
        public string AcademicSession { get; set; } = string.Empty;
        
        [Required]
        public int Semester { get; set; }
        
        public IFormFile? PdfFile { get; set; }
    }
}