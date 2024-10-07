using Blog_Engine_2;
using Blog_Engine_2.Objects;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Default");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Context>(options => options.UseSqlServer(connectionString));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/home/authorization";
                options.AccessDeniedPath = options.LoginPath;
                options.LogoutPath = "/home/logout";
            });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Constants.AuthorPolicy,
        config => config.RequireClaim(ClaimTypes.Role, RoleUser.Author.ToString()))

    .AddPolicy(Constants.ViewerPolicy,
        config => config.RequireClaim(ClaimTypes.Role, RoleUser.Viewer.ToString()));
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=PostCreate}/{id?}");

app.Run();
