using GestaoObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoObras.Data.Configurations;

public class MovimentoMaterialConfiguration : IEntityTypeConfiguration<MovimentoMaterial>
{
    public void Configure(EntityTypeBuilder<MovimentoMaterial> b)
    {
        b.ToTable("MovimentosMaterial");
        b.HasKey(x => x.Id);

        b.Property(x => x.Quantidade).IsRequired();
        b.Property(x => x.DataHora).IsRequired();

        b.HasOne(x => x.Obra)
         .WithMany(o => o.MovimentosMaterial)
         .HasForeignKey(x => x.ObraId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Material)
         .WithMany(m => m.Movimentos)
         .HasForeignKey(x => x.MaterialId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.DataHora);
    }
}