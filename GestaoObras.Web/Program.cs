using GestaoObras.Data.Context;
using GestaoObras.Web.Infrastructure.Binding;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC + binder tolerante a ponto/vírgula para doubles
builder.Services.AddControllersWithViews(o =>
{
    o.ModelBinderProviders.Insert(0, new FlexibleDoubleModelBinderProvider());
});

// Connection string tem de vir de env vars/user-secrets
var cs = builder.Configuration.GetConnectionString("ObrasDb");
if (string.IsNullOrWhiteSpace(cs))
    throw new InvalidOperationException(
        "Connection string 'ObrasDb' não encontrada (ou vazia). " +
        "Define via variável de ambiente ConnectionStrings__ObrasDb ou user-secrets.");

builder.Services.AddDbContext<ObrasDbContext>(opt => opt.UseNpgsql(cs));

var app = builder.Build();

// Seed/Migrate apenas quando a flag estiver true
var runSeed = builder.Configuration.GetValue<bool>("ApplyMigrationsOnStartup");
if (runSeed)
{
    try { await GestaoObras.Web.Seed.DataSeeder.SeedAsync(app); }
    catch (Exception ex) { app.Logger.LogError(ex, "[SEED] Falhou"); }
}

// pipeline default do template…
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();