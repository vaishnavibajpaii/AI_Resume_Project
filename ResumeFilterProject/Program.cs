using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using ResumeFilterProject.Data; // Apke DbContext ka namespace

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddControllersWithViews();

//Entity Framework DbContext registration (SQL Server example)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//File upload ke liye size limit optional (agar chahiye to)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB limit
});

var app = builder.Build();

//Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Resume}/{action=Upload}/{id?}");

app.Run();
