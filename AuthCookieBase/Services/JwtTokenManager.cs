using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthCookieBase
{
    public class JwtTokenManager: ITokenManager
    {
        public ClaimsPrincipal GetClaimsPrincipal(string token)
        {
            //✅Work 
            //روش اول
            //در صورتی مناسب است که به تنظیمات توکن در زمان ساخت توکن در پروژه ی api دسترسی نداریم
            //بدون اعتبارسنجی فقط ClaimsPrincipal را برمی گرداند

            //var handler = new JwtSecurityTokenHandler();
            //var jwtToken = handler.ReadJwtToken(token);

            //if (jwtToken == null)
            //    throw new ArgumentException("Invalid JWT Token");

            //////در توکن Jwt کلایم تایپ Name به unique_name نگاشت می شود
            ////// تبدیل `unique_name` به `ClaimTypes.Name`

            //////برای این که اتریبیوت [Authorize] در کنترلر رول را شناسایی کند باید نوع کلایم System.Security.Claims.ClaimTypes.Role باشد نه role پس تبدیل میکنیم
            //var claimsIdentity = new ClaimsIdentity(jwtToken.Claims.Select(c =>
            //    c.Type == "role" ? new Claim(ClaimTypes.Role, c.Value) :
            //    c.Type == "unique_name" ? new Claim(ClaimTypes.Name, c.Value) :
            //    c.Type == "nameid" ? new Claim(ClaimTypes.NameIdentifier, c.Value) :
            //    c
            //), CookieAuthenticationDefaults.AuthenticationScheme);

            //claimsIdentity.AddClaim(new Claim("access_Token", token));
            //return new ClaimsPrincipal(claimsIdentity);

            //Or
            //روش دوم
            //در صورتی مناسب است که به تنظیمات توکن در زمان ساخت توکن در پروژه ی api دسترسی داریم
            //با اعتبارسنجی ClaimsPrincipal را برمی گرداند

            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            //در صورتی که در زمان ساخت توکن موارد زیر تنظیم نشده باشند
            //validationParameters.ValidateAudience = false;
            //validationParameters.ValidateIssuer = false;
            //validationParameters.ValidateIssuerSigningKey = false;

            validationParameters.ValidAudience = "http://localhost:5000".ToLower();
            validationParameters.ValidIssuer = "http://localhost:5000".ToLower();
            // فقط اجازه الگوریتم HS256 برای امضا
            validationParameters.ValidAlgorithms = new List<string>{ SecurityAlgorithms.HmacSha256 };
            validationParameters.IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes("Hello, I am Mahnaz Azad, Software Engineer."));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out validatedToken);

            var identity = new ClaimsIdentity(principal.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim("access_Token", token));
            principal = new ClaimsPrincipal(identity);

            return principal;
        }
    }

    //برای این که این اعتبارسنجی توکن درست انجام شود باید با پروژه ی api هماهنگ شود

    //در پروژه ی api

    //تنظیمات در فایل appsettings.json در پروژه ی api
    //    "JWT": {
    //  "Secret": "Hello, I am Mahnaz Azad, Software Engineer.",
    //  "ValidAudience": "http://localhost:5000",
    //  "ValidIssuer": "http://localhost:5000"
    //},

    //تنظیمات توکن در زمان ساخت توکن در پروژه ی api
    //var tokenDescriptor = new SecurityTokenDescriptor
    //{
    //    Subject = authClaims,
    //    Expires = DateTime.UtcNow.AddDays(7),
    //    Issuer = _jwt.ValidIssuer,
    //    Audience = _jwt.ValidAudience,
    //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret)),
    //                SecurityAlgorithms.HmacSha256)
    //};
}
