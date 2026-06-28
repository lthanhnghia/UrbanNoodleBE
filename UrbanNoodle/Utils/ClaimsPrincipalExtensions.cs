using System.Security.Claims;

namespace UrbanNoodle.Utils
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetAccountId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst("id")?.Value;
            return int.TryParse(userIdClaim, out int id) ? id : null;
        }
    }
}
