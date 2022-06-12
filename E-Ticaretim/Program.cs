using Microsoft.EntityFrameworkCore;
using System.Globalization;
var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<E_Ticaretim.Models.E_TicaretimContext>();
builder.Services.AddDbContext<E_Ticaretim.Models.E_TicaretimContext>(x => x.UseSqlServer("Data Source=DESKTOP-Q0IKKRN\\SQLEXPRESS;Initial Catalog=E-Ticaret;User Id=sa;Password=1234"));
builder.Services.AddDbContext<E_Ticaretim.Areas.Admin.Models.UserContext>(x => x.UseSqlServer("Data Source=DESKTOP-Q0IKKRN\\SQLEXPRESS;Initial Catalog=E-Ticaret;User Id=sa;Password=1234"));


builder.Services.AddSession();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseSession();

CultureInfo culture = new CultureInfo("tr-TR");
culture.NumberFormat.NumberDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();



app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"

);


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
