using classroom_api.Models;
using classroom_api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//var connectionString = builder.Configuration.GetConnectionString("classroom_apiContextConnection");
//builder.Services.AddDbContext<ClassroomapiContext>(options =>
//         options.UseNpgsql(connectionString));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ClassroomapiContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddScoped<IHttpClientService, HttpClientService>();
builder.Services.AddScoped<ITSUService, TSUService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
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
//using (var db = new ClassroomapiContext())
//{
//    db.Users.Add(new UserModel
//    {
//        Name = "admin",
//    });
//    db.SaveChanges();
//}
//using (var db = new ClassroomapiContext())
//{
//    List<PermissionModel> permissions = new List<PermissionModel> {
//        new PermissionModel { Action = "GetFaculties" }
//    };
//    db.Permissions.AddRange(permissions);
//    db.Roles.AddRange(
//        new RoleModel { Name = "Admin", Permissions = permissions },
//        new RoleModel { Name = "Admin2", Permissions = permissions }
//        );
//    db.SaveChanges();
//}
app.Run();
