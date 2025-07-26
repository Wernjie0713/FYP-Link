using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FYP_Link.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string MatricNumber { get; set; } = string.Empty;

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        // Supervisor selection and approval
        public int? SupervisorId { get; set; }
        [ForeignKey("SupervisorId")]
        public Lecturer? Supervisor { get; set; }

        public string ApprovalStatus { get; set; } = "Pending";

        public DateTime? UpdatedAt { get; set; }

        // Navigation property for proposals
        public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
    }
}