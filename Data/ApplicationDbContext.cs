using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthMvc.Models;

namespace AuthMvc.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // ----- Tables de ton application -----
        public DbSet<AuthMvc.Models.TaskItem> TaskItem { get; set; } = default!;
        public DbSet<AuthMvc.Models.Commentaire> Commentaire { get; set; } = default!;
        public DbSet<AuthMvc.Models.Vote> Vote { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Appliquer la configuration d'Identity de base
            base.OnModelCreating(builder);

            // ----- Configuration de la table Vote -----
            builder.Entity<Vote>()
                .HasIndex(v => new { v.TaskItemId, v.UserId })
                .IsUnique(); // Un utilisateur ne peut voter qu'une seule fois par t�che

            builder.Entity<Vote>()
                .HasOne(v => v.TaskItem)
                .WithMany(t => t.Votes)
                .HasForeignKey(v => v.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade); // Supprime les votes si la t�che est supprim�e

            builder.Entity<Vote>()
                .HasOne(v => v.User)
                .WithMany() // Si tu veux que l�utilisateur ait une liste de Votes, utilise .WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict); // �vite de supprimer les votes si l�utilisateur est supprim�

            // ----- Configuration de la table TaskItem -----
            builder.Entity<TaskItem>()
                .HasOne(t => t.Owner)
                .WithMany(u => u.Taches) // N�cessite une collection Taches dans ApplicationUser
                .HasForeignKey(t => t.OwnerId)
                .OnDelete(DeleteBehavior.Cascade); // Supprime les t�ches si l�utilisateur est supprim�

            // ----- Configuration de la table Commentaire -----
builder.Entity<Commentaire>()
    .HasOne(c => c.TaskItem)
    .WithMany(t => t.Commentaires)
    .HasForeignKey(c => c.TaskItemId)
    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Commentaire>()
                .HasOne(c => c.User)
                .WithMany() // Ou .WithMany(u => u.Commentaires)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); // L'utilisateur ne doit pas �tre supprim� automatiquement
        }
    }
}
