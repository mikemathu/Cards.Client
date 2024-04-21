using Cards.Client.Handler;
using Cards.Client.Repositories;
using Cards.Client.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
      .AddCookie(options =>
      {
          options.LoginPath = "/Auth/Login"; // Set the login page path
      });

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ITokenRepository, TokenRepository>();

builder.Services.AddHttpClient<IAuthRepository, AuthRepository>(
    c => c.BaseAddress = new Uri("https://localhost:7265/api/authentication/"));

builder.Services.AddScoped<LoginHandler>();

builder.Services.AddHttpClient<ICardRepository, CardRepository>(
                    c => c.BaseAddress = new Uri("https://localhost:7265/api/appUsers/"))
                    .AddHttpMessageHandler<LoginHandler>();

builder.Services.AddSingleton<ITokenRepository, TokenRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
