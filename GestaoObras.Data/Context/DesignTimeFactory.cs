using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GestaoObras.Data.Context;

public sealed class DesignTimeFactory : IDesignTimeDbContextFactory<ObrasDbContext>
{
    public ObrasDbContext CreateDbContext(string[] args)
    {
        // Lê appsettings do projeto Web (onde está a connection string)
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "GestaoObras.Web");

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("ObrasDb")
                 ?? throw new InvalidOperationException("Connection string 'ObrasDb' não encontrada.");

        var options = new DbContextOptionsBuilder<ObrasDbContext>()
            .UseNpgsql(cs, o =>
            {
                // Garante que as migrações ficam neste assembly (Data)
                o.MigrationsAssembly(typeof(ObrasDbContext).Assembly.FullName);
            })
            .Options;

        return new ObrasDbContext(options);
    }
}