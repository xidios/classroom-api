using classroom_api.Models;
using classroom_api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//var connectionString = builder.Configuration.GetConnectionString("classroom_apiContextConnection");builder.Services.AddDbContext<ClassroomapiContext>(options =>
//    options.UseNpgsql(connectionString));builder.Services.AddDbContext<ClassroomapiContext>(options =>
//    options.UseNpgsql(connectionString));builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ClassroomapiContext>();
// Add services to the container.

builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("classroom_apiContextConnection");
builder.Services.AddDbContext<ClassroomapiContext>(options =>
         options.UseNpgsql(connectionString));


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapRazorPages();
app.MapControllers();
//using( var db = new ClassroomapiContext())
//{
//    db.Students.Add(new StudentModel
//    {
//        Name = "xqc",
//        AccountId = "104780416574104474334",
//        Email = "redbull-8@bk.ru"
//    });
//    db.SaveChanges();
//}
app.Run();
