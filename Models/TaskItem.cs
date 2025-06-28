using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthMvc.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        public required string Titre { get; set; }

        public required string Description { get; set; }

        public DateTime DateEcheance { get; set; }

        public StatutTache Statut { get; set; }

        public PrioriteTache Priorite { get; set; }

        [Required]
        public required string OwnerId { get; set; }

        public required ApplicationUser Owner { get; set; }

        // Fichier upload� (non mapp� dans la base de donn�es)
        [NotMapped]
        [Display(Name = "Fichier � importer")]
        public IFormFile? Fichier { get; set; }

        // Liste des votes re�us
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();

        // Liste des commentaires li�s
        public ICollection<Commentaire> Commentaires { get; set; } = new List<Commentaire>();

        public int NombreVotes => Votes?.Count ?? 0;

        public string? CheminFichier { get; set; }
    }
}
