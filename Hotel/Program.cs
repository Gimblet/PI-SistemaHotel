using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Hotel.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HotelContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelContext") ?? throw new InvalidOperationException("Connection string 'HotelContext' not found.")));

builder.Services.AddDistributedMemoryCache();  // Usa una caché en memoria para la sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Tiempo de espera de la sesión
    options.Cookie.HttpOnly = true;  // Asegura que la cookie sea solo accesible por HTTP
    options.Cookie.IsEssential = true;  // Hace que la cookie sea esencial para el funcionamiento de la aplicación
});
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Habitacion/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
