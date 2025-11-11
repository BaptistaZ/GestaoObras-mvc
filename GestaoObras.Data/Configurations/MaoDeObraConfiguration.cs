using GestaoObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoObras.Data.Configurations;

public class MaoDeObraConfiguration : IEntityTypeConfiguration<MaoDeObra>
{
    public void Configure(EntityTypeBuilder<MaoDeObra> b)
    {
        b.ToTable("MaosDeObra", tb =>
        {
            // Horas não negativas
            tb.HasCheckConstraint("CK_MaosDeObra_Horas_Pos", "HorasTrabalhadas >= 0");
        });

        b.HasKey(x => x.Id);

        b.Property(x => x.Nome).IsRequired().HasMaxLength(120);
        b.Property(x => x.HorasTrabalhadas).HasPrecision(9, 2);
        b.Property(x => x.DataHora).IsRequired().HasDefaultValueSql("now() at time zone 'utc'");

        b.HasOne(x => x.Obra)
         .WithMany(o => o.MaosDeObra)
         .HasForeignKey(x => x.ObraId)
         .OnDelete(DeleteBehavior.Cascade);

        // Acesso típico: por Obra e Data
        b.HasIndex(x => new { x.ObraId, x.DataHora });
    }
}