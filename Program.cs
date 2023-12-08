using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using WebNoticias.Models.Entities;
using WebNoticias.Repositories;

var builder = WebApplication.CreateBuilder(args);

string? Db = builder.Configuration.GetConnectionString("DbConnection");
builder.Services.AddDbContext<ProyectonoticiasContext>(x => x.UseMySql(Db, ServerVersion.AutoDetect(Db)));
builder.Services.AddMvc(); 

builder.Services.AddTransient<NoticiasRepository>();
builder.Services.AddTransient<CategoriasRepository>();
builder.Services.AddTransient<Repository<Usuario>>();     

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x =>
{
    x.AccessDeniedPath = "/Home/Denied";
    x.LoginPath = "/Home/Login";  
    x.LogoutPath = "/Home/Logout";   
    x.ExpireTimeSpan = TimeSpan.FromMinutes(15); //Tiempo en la que la cookie esta activa.
    x.Cookie.Name = "noticiaCookie";

});

var app = builder.Build();

app.UseStaticFiles(); 
app.UseFileServer();
app.MapControllerRoute(
name: "areas",
pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapDefaultControllerRoute();

app.Run();