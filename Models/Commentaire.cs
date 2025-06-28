using System.ComponentModel.DataAnnotations;

namespace AuthMvc.Models
{
    public class Commentaire
    {
        public int Id { get; set; }

        [Required]
        public required string Contenu { get; set; }

        public DateTime DateCommentaire { get; set; } = DateTime.UtcNow;

        [Required]
        public int TaskItemId { get; set; }

        public required TaskItem TaskItem { get; set; }

        [Required]
        public required string UserId { get; set; }

        public required ApplicationUser User { get; set; }
    }
}
