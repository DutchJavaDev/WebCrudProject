using WebCrudProject.Services.ORM;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");

var types = new Type[] {  };

var sqlOrm = new SqlServerORM();
await sqlOrm.InitAsync(connectionString, types);

// Add services to the container.
builder.Services.AddSingleton(sqlOrm);

builder.Services.AddScoped((context) => 
{
    var iorm = context.GetService<SqlServerORM>();
    return iorm.GetObjectContext();
});

builder.Services.AddControllersWithViews();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
