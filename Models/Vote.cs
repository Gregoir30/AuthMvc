using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthMvc.Models
{
    public class Vote
    {
        public int Id { get; set; }

        [Required]
        public int TaskItemId { get; set; }

        public required TaskItem TaskItem { get; set; }

        [Required]
        public required string UserId { get; set; }

        public required ApplicationUser User { get; set; }

        public DateTime DateVote { get; set; } = DateTime.UtcNow;
    }
}
