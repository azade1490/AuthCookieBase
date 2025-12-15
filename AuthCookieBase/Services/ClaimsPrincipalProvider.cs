using Microsoft.AspNetCore.Authentication.Cookies;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace AuthCookieBase
{
    public class ClaimsPrincipalProvider : IClaimsPrincipalProvider
    {
        //روش اول
        public ClaimsPrincipal GetClaimsPrincipal(string token)
        {
            //بدون اعتبارسنجی فقط ClaimsPrincipal را برمی گرداند
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            if (jwtToken == null)
                throw new ArgumentException("Invalid JWT Token");

            ////در توکن Jwt کلایم تایپ Name به unique_name نگاشت می شود
            //// تبدیل `unique_name` به `ClaimTypes.Name`

            ////برای این که اتریبیوت [Authorize] در کنترلر رول را شناسایی کند باید نوع کلایم System.Security.Claims.ClaimTypes.Role باشد نه role پس تبدیل میکنیم
            var claimsIdentity = new ClaimsIdentity(jwtToken.Claims.Select(c =>
                c.Type == "role" ? new Claim(ClaimTypes.Role, c.Value) :
                c.Type == "unique_name" ? new Claim(ClaimTypes.Name, c.Value) :
                c.Type == "nameid" ? new Claim(ClaimTypes.NameIdentifier, c.Value) :
                c
            ), CookieAuthenticationDefaults.AuthenticationScheme);

            claimsIdentity.AddClaim(new Claim("access_Token", token));
            return new ClaimsPrincipal(claimsIdentity);

        }

        //روش دوم
        public ClaimsPrincipal ParseClaimsFromJwt(string token)
        {
            var payload = token.Split('.')[1];
            var jsonBytes = Convert.FromBase64String(payload);
            var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            var listClaim = claims.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));

            var claimsIdentity = new ClaimsIdentity(listClaim, CookieAuthenticationDefaults.AuthenticationScheme);

            claimsIdentity.AddClaim(new Claim("access_Token", token));
            return new ClaimsPrincipal(claimsIdentity);
        }


    }
}
