using GestaoObras.Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var cs = builder.Configuration.GetConnectionString("ObrasDb")
         ?? throw new InvalidOperationException("Connection string 'ObrasDb' não encontrada.");
builder.Services.AddDbContext<ObrasDbContext>(opt => opt.UseNpgsql(cs));

var app = builder.Build();
// pipeline default do template…
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();