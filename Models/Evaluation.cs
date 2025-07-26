using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FYP_Link.Models
{
    public class Evaluation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Result { get; set; } = string.Empty;

        public string Comments { get; set; } = string.Empty;

        [Required]
        public int ProposalId { get; set; }
        [ForeignKey("ProposalId")]
        public Proposal Proposal { get; set; } = null!;

        [Required]
        public int EvaluatorId { get; set; }
        [ForeignKey("EvaluatorId")]
        public Lecturer Evaluator { get; set; } = null!;

        [Required]
        public bool IsCurrent { get; set; } = true;
    }
}