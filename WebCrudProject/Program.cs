using WebCrudProject.Auth;
using WebCrudProject.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddScoped(p => { return new UserDbService(connectionString); });
builder.Services.AddScoped(p => { return new UserDocumentDbService(connectionString); });
builder.Services.AddScoped(p => { return new UserFileService(connectionString); });

var app = builder.Build();

app.Use(UploadMiddelware.UploadFilter);
app.Use(SignInMiddelware.CheckSignIn);

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
