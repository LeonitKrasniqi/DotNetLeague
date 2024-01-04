using DotNetLeague.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetLeague.API.Data
{
    public class DotNetLeagueDbContext : DbContext
    {

        public DotNetLeagueDbContext(DbContextOptions<DotNetLeagueDbContext> dbContextOptions):base(dbContextOptions)
        {
            
        }


        public DbSet<Team> Teams { get; set; }
        public DbSet<Group> Groups { get; set; }

        public DbSet<League> League { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<League>()
                .HasKey(l => new { l.TeamId, l.GroupId });

            modelBuilder.Entity<League>()
                .HasOne(l => l.Team)
                .WithMany(t => t.League)
                .HasForeignKey(l => l.TeamId);

            modelBuilder.Entity<League>()
                .HasOne(l => l.Group)
                .WithMany(g => g.League)
                .HasForeignKey(l => l.GroupId);

            modelBuilder.Entity<League>()
                .HasOne(l => l.Team)
                .WithMany()
                .HasForeignKey(l => l.TeamId)
                .HasConstraintName("FK_League_Team")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<League>()
                .HasOne(l => l.Group)
                .WithMany(g => g.League)
                .HasForeignKey(l => l.GroupId)
                .HasConstraintName("FK_League_Group")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Match>()
                .HasKey(m => m.MatchId);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Group)
                .WithMany(x=>x.Matches)
                .HasForeignKey(m => m.GroupId)
                .HasConstraintName("FK_Match_Group")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Team1)
                .WithMany()
                .HasForeignKey(m => m.Team1Id)
                .HasConstraintName("FK_Match_Team1")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Team2)
                .WithMany()
                .HasForeignKey(m => m.Team2Id)
                .HasConstraintName("FK_Match_Team2")
                .OnDelete(DeleteBehavior.NoAction);


            base.OnModelCreating(modelBuilder);
        }

    }
}
