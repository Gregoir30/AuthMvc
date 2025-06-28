using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace AuthMvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Une liste des tâches créées par l’utilisateur
        public ICollection<TaskItem> Taches { get; set; } = new List<TaskItem>();

        // (optionnel) Une liste des commentaires postés par l'utilisateur
        public ICollection<Commentaire> Commentaires { get; set; } = new List<Commentaire>();

        // (optionnel) Une liste des votes donnés par l'utilisateur
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
