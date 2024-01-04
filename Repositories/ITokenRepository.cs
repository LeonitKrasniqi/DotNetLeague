using Microsoft.AspNetCore.Identity;

namespace DotNetLeague.API.Repositories
{
    public interface ITokenRepository
    {
       string CreateJWTToken(IdentityUser user, List<string>roles);
    }
}
