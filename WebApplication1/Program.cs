using Microsoft.AspNetCore.Authentication.Cookies;
using WebApplication1.DAL.Context;
using WebApplication1.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// HurdaciContext hizmetini ekleyin
builder.Services.AddDbContext<HurdaciContext>();

// Cookie Authentication ekleyin
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";  // Yetkisiz eri�im durumunda y�nlendirme
        options.LogoutPath = "/Layout/Index"; // ��k�� i�lemi
        options.AccessDeniedPath = "/Login/Index"; // Eri�im engellenmi�se y�nlendirme
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Oturum s�resi
        options.SlidingExpiration = true;
        options.Cookie.Expiration = null;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Sadece HTTPS ile �erez g�nderilir
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

var app = builder.Build();

// Middleware yap�land�rmalar�
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Kimlik do�rulama ve yetkilendirme middleware'leri
app.UseAuthentication();
app.UseAuthorization();

// Varsay�lan admin kayd�n� olu�tur
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HurdaciContext>();
    PasswordHelper.SeedAdmin(context); // SeedAdmin �a�r�s�
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Index}/{id?}");  // Varsay�lan rota, Default/Index

app.Run();
