using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Repository.Interface
{
    public interface ITokenRepository
    {
        string CreateJwtToken(IdentityUser user, List<string> roles);
    }
}
