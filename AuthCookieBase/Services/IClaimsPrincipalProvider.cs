using System.Security.Claims;

namespace AuthCookieBase
{
    public interface IClaimsPrincipalProvider
    {
        ClaimsPrincipal GetClaimsPrincipal(string token);
    }
}
