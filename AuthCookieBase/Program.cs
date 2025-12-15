using AuthCookieBase.Components;
using AuthCookieBase;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

// برای استفاده از @attribute [Authorize] باید کوکی احراز هویت ایجاد شده باشد 

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
 .AddCookie(options =>
 {
     //مربوط به Https
     options.Cookie.SecurePolicy = CookieSecurePolicy.None;//فقط برای توسعه None می گذاریم
     options.LogoutPath = "/account/login";     //حتما باید در مسیر /account/ باشد
     options.AccessDeniedPath = "/account/accessDenied";

     //موارد زیر پیش فرض ست می شوند
     //options.LoginPath = "/"; // مسیر صفحه لاگین
     //options.Cookie.HttpOnly = true;

     //Lax :کوکی فقط در درخواست‌های "safe"(مانند GET) ارسال می‌شود.
     //Strict:کوکی فقط وقتی ارسال می‌شود که درخواست از همان دامنه باشد(حتی اگر لینک خارجی به سایت  زده شود، ارسال نمی‌شود).
     //options.Cookie.SameSite = SameSiteMode.Lax;

     //کوکی‌های HTTP عموماً پورت-محور نیستند. این بدان معناست که یک کوکی تنظیم شده برای یک دامنه مشخص معمولاً به هر پورتی در آن دامنه ارسال می‌شود.
     //options.Cookie.Domain = "localhost";
     //options.Cookie.Path = "/";
 });

//سرویس IHttpContextAccessor با طول عمر Singleton  اضافه می‌شود
//با اینکه طول عمر آن Singleton است، خاصیت HttpContext داخل آن، در هر درخواست متفاوت خواهد بود و به طور صحیح با کانتکست فعلی تنظیم می‌شود.
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorizationCore(); // اضافه کردن مجوزها

//برای این که در کامپوننت‌ غیر صفحه‌ای اطلاعات احراز هویت کامپوننت پدر را بخواند مثلا وقتی که از تگ AuthorizeView استفاده میکنیم بدون این خط کد خطا می دهد 
//یا هر کامپوننت غیر صفحه ای که خودمان نوشته باشیم
builder.Services.AddCascadingAuthenticationState();//به جای استفاده از تگ <CascadingAuthenticationState> در Routes.razor
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();//برای احرازهویت بدون نوشتن این خط در AuthorizeView کوکی احراز هویت بررسی می شود ولی با این خط متد GetAuthenticationStateAsync از CustomAuthenticationStateProvider

builder.Services.AddScoped<IClaimsPrincipalProvider, ClaimsPrincipalProvider>();

/////IHttpClientFactory استفاده از 
//سرویس HttpClient با طول عمر Singleton  اضافه می‌شود
builder.Services.AddHttpClient("MyHttpClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/");
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
});


var app = builder.Build();

// UseAuthentication باید بعد از app.UseRouting() و قبل از UseEndpoints باشد
app.UseAuthentication(); // ابتدا احراز هویت اجرا شود
app.UseAuthorization();  // سپس بررسی مجوزها انجام شود

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}


app.UseAntiforgery();

app.MapStaticAssets();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
