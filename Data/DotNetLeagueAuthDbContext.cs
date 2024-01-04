using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotNetLeague.API.Data
{
    public class DotNetLeagueAuthDbContext : IdentityDbContext
    {
        public DotNetLeagueAuthDbContext(DbContextOptions<DotNetLeagueAuthDbContext> options) : base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var adminId = "ecbbf4be-7ff4-4fba-b104-d63991b6f0b6";
            var userId = "6b3f9ad8-4740-4a23-b6e8-6177d8b7c31c";


            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = adminId,
                    ConcurrencyStamp=adminId,
                    Name="Admin",
                    NormalizedName="Admin".ToUpper(),
                },
                new IdentityRole {
                    Id = userId,
                    ConcurrencyStamp=userId,
                    Name="User",
                    NormalizedName="User".ToUpper(),
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
