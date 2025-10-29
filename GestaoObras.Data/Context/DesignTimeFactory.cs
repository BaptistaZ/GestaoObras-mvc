using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GestaoObras.Data.Context;

public class DesignTimeFactory : IDesignTimeDbContextFactory<ObrasDbContext>
{
    public ObrasDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "GestaoObras.Web");
        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("ObrasDb")
                 ?? throw new InvalidOperationException("Connection string 'ObrasDb' não encontrada.");

        var options = new DbContextOptionsBuilder<ObrasDbContext>()
            .UseNpgsql(cs)
            .Options;

        return new ObrasDbContext(options);
    }
}