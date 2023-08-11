using WebCrudProject.Auth;
using WebCrudProject.Auth.Models;
using WebCrudProject.Auth.Services;
using WebCrudProject.Auth.Services.Interfaces;
using WebCrudProject.Services.Email;
using WebCrudProject.Services.Email.Interfaces;
using WebCrudProject.Services.ORM;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession();

var connectionString = "Data Source=webcrudproject-server.database.windows.net,1433;Initial Catalog=webcrudproject-database;User ID=webcrudproject-server-admin;Password=U5U5B70088J1E3Y1$";

var types = new Type[] 
{ 
    typeof(ELUser), 
    typeof(ELJwtSession)
};

var sqlOrm = new SqlServerORM();
await sqlOrm.InitAsync(connectionString, types);

// Add services to the container.
builder.Services.AddSession();

builder.Services.AddSingleton(sqlOrm);

builder.Services.AddScoped((context) => 
{
    var iorm = context.GetService<SqlServerORM>();
    return iorm.GetObjectContext();
});

builder.Services.AddScoped<ISessionService,SessionService>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<IELMailService, ELMailService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(AuthenticationMiddelWare.SessionResolve);

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
