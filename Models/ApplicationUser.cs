using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace AuthMvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Une liste des t�ches cr��es par l�utilisateur
        public ICollection<TaskItem> Taches { get; set; } = new List<TaskItem>();

        // (optionnel) Une liste des commentaires post�s par l'utilisateur
        public ICollection<Commentaire> Commentaires { get; set; } = new List<Commentaire>();

        // (optionnel) Une liste des votes donn�s par l'utilisateur
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
