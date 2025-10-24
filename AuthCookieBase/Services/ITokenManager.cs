using System.Security.Claims;

namespace AuthCookieBase
{
    public interface ITokenManager
    {
        ClaimsPrincipal GetClaimsPrincipal(string token);
    }
}
