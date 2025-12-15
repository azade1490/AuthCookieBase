
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
namespace AuthCookieBase
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private AuthenticationState _currentUser;

        public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            //Auto Login با استفاده از کوکی احراز هویت
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                _currentUser = new AuthenticationState(_httpContextAccessor.HttpContext.User);//Login currentUser 
                return;
            }
            else
            {
                _currentUser = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));//Login Anonymous User
                return;
            }
        }
        //هرجایی که وضعیت احراز هویت بررسی می شود خودبه خود فراخوانی می شود مثلا در تگ AuthorizeView 
        public override async Task<AuthenticationState> GetAuthenticationStateAsync() => await Task.FromResult(_currentUser);
    }
}