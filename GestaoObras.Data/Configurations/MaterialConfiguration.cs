using GestaoObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoObras.Data.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> b)
    {
        b.ToTable("Materiais");
        b.HasKey(x => x.Id);

        b.Property(x => x.Nome).IsRequired().HasMaxLength(120);
        b.Property(x => x.Descricao).HasMaxLength(500);
        b.Property(x => x.StockDisponivel).IsRequired();

        b.HasIndex(x => x.Nome).IsUnique();
    }
}