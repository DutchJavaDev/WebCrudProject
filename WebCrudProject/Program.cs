using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.Security.Claims;
using WebCrudProject.Models;
using WebCrudProject.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = "data source=LAPTOP-BORIS;initial catalog=webcrudproject;persist security info=True;Integrated Security=SSPI;";

builder.Services.AddScoped(p => { return new UserDbService(connectionString); });

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

app.Use(async (context, next) =>
{
    if (!context.User.Identity.IsAuthenticated && context.Request.Path.Value != "/Auth/Index")
    {
        var hasCookie = context.Request.Cookies.TryGetValue("cookies", out var userAuth);

        if (hasCookie)
        {
            var model = JsonConvert.DeserializeObject<AuthenticationModel>(userAuth);
            var claims = new[] {
                    new Claim(ClaimTypes.Email, model.Email, ClaimTypes.Name, model.Email),
                };
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            context.User = principal;
        }
        else
        { 
            context.Response.Redirect($"/Auth/Index?rtnUrl={context.Request.Path}");
        }

    }
   
    await next();

});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
