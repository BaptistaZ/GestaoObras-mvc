using GestaoObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoObras.Data.Context;

public sealed class ObrasDbContext : DbContext
{
    public ObrasDbContext(DbContextOptions<ObrasDbContext> options) : base(options) { }

    // DbSets
    public DbSet<Cliente>          Clientes           => Set<Cliente>();
    public DbSet<Obra>             Obras              => Set<Obra>();
    public DbSet<Material>         Materiais          => Set<Material>();
    public DbSet<MovimentoMaterial> MovimentosMaterial => Set<MovimentoMaterial>();
    public DbSet<MaoDeObra>        MaosDeObra         => Set<MaoDeObra>();
    public DbSet<Pagamento>        Pagamentos         => Set<Pagamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica automaticamente todas as IEntityTypeConfiguration<>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ObrasDbContext).Assembly);
    }
}