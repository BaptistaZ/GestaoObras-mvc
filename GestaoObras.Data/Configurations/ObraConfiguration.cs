using GestaoObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoObras.Data.Configurations;

public class ObraConfiguration : IEntityTypeConfiguration<Obra>
{
    public void Configure(EntityTypeBuilder<Obra> b)
    {
        b.ToTable("Obras");
        b.HasKey(x => x.Id);

        b.Property(x => x.Nome).IsRequired().HasMaxLength(150);
        b.Property(x => x.Descricao).IsRequired().HasMaxLength(1000);
        b.Property(x => x.Morada).IsRequired().HasMaxLength(200);
        b.Property(x => x.Latitude).HasPrecision(10, 6);
        b.Property(x => x.Longitude).HasPrecision(10, 6);
        b.Property(x => x.Ativa).IsRequired();

        b.HasOne(x => x.Cliente)
         .WithMany(c => c.Obras)
         .HasForeignKey(x => x.ClienteId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.ClienteId, x.Nome });
    }
}