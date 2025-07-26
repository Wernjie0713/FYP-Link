using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYP_Link.Data;
using FYP_Link.Models;
using System.Threading.Tasks;
using System.Linq;

namespace FYP_Link.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class CommitteeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommitteeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{programId}/members")]
        public async Task<IActionResult> GetCommitteeMembers(int programId)
        {
            var members = await _context.CommitteeMemberships
                .Where(cm => cm.AcademicProgramId == programId)
                .Include(cm => cm.Lecturer)
                .Select(cm => cm.Lecturer)
                .ToListAsync();

            return Ok(members);
        }

        [HttpGet("available-lecturers")]
        public async Task<IActionResult> GetAvailableLecturers()
        {
            var assignedLecturerIds = await _context.CommitteeMemberships
                .Select(cm => cm.LecturerId)
                .ToListAsync();

            var availableLecturers = await _context.Lecturers
                .Where(l => !assignedLecturerIds.Contains(l.Id))
                .ToListAsync();

            return Ok(availableLecturers);
        }

        [HttpPost("members")]
        public async Task<IActionResult> AddCommitteeMember([FromBody] CommitteeMembership membership)
        {
            // Check if lecturer is already assigned to any committee
            var existingMembership = await _context.CommitteeMemberships
                .FirstOrDefaultAsync(cm => cm.LecturerId == membership.LecturerId);

            if (existingMembership != null)
            {
                return BadRequest("This lecturer is already assigned to a committee.");
            }

            // Add the committee membership
            _context.CommitteeMemberships.Add(membership);
            await _context.SaveChangesAsync();

            // Get the lecturer's user account
            var lecturer = await _context.Lecturers
                .Include(l => l.ApplicationUser)
                .FirstOrDefaultAsync(l => l.Id == membership.LecturerId);

            if (lecturer?.ApplicationUser != null)
            {
                // Add the Committee role if they don't already have it
                if (!await _userManager.IsInRoleAsync(lecturer.ApplicationUser, "Committee"))
                {
                    await _userManager.AddToRoleAsync(lecturer.ApplicationUser, "Committee");
                }
            }

            return Ok();
        }

        [HttpDelete("members/{lecturerId}/{programId}")]
        public async Task<IActionResult> RemoveCommitteeMember(int lecturerId, int programId)
        {
            var membership = await _context.CommitteeMemberships
                .FirstOrDefaultAsync(cm => cm.LecturerId == lecturerId && cm.AcademicProgramId == programId);

            if (membership == null)
            {
                return NotFound();
            }

            _context.CommitteeMemberships.Remove(membership);
            await _context.SaveChangesAsync();

            // Check if the lecturer is still a member of any other committees
            var hasOtherCommittees = await _context.CommitteeMemberships
                .AnyAsync(cm => cm.LecturerId == lecturerId);

            if (!hasOtherCommittees)
            {
                // Remove the Committee role if they're not on any other committees
                var lecturer = await _context.Lecturers
                    .Include(l => l.ApplicationUser)
                    .FirstOrDefaultAsync(l => l.Id == lecturerId);

                if (lecturer?.ApplicationUser != null)
                {
                    await _userManager.RemoveFromRoleAsync(lecturer.ApplicationUser, "Committee");
                }
            }

            return Ok();
        }
    }
}