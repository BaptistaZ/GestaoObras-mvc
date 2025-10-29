using GestaoObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoObras.Data.Configurations;

public class MaoDeObraConfiguration : IEntityTypeConfiguration<MaoDeObra>
{
    public void Configure(EntityTypeBuilder<MaoDeObra> b)
    {
        b.ToTable("MaosDeObra");
        b.HasKey(x => x.Id);

        b.Property(x => x.Nome).IsRequired().HasMaxLength(120);
        b.Property(x => x.HorasTrabalhadas).HasPrecision(9, 2);
        b.Property(x => x.DataHora).IsRequired();

        b.HasOne(x => x.Obra)
         .WithMany(o => o.MaosDeObra)
         .HasForeignKey(x => x.ObraId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.DataHora);
    }
}