using System.Security.Claims;
using SM_API.Models;

namespace SM_API.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user, List<string> roles);

        string Old1_GenerateToken(string userId, string username, string role);
        string Old2_GenerateToken(User user, List<string> roles, List<string> permissions);
    }
}
