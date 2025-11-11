using GestaoObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoObras.Data.Configurations;

public class ObraConfiguration : IEntityTypeConfiguration<Obra>
{
    public void Configure(EntityTypeBuilder<Obra> b)
    {
        b.ToTable("Obras", tb =>
        {
            tb.HasCheckConstraint("CK_Obras_Lat_Range", "\"Latitude\" BETWEEN -90 AND 90");
            tb.HasCheckConstraint("CK_Obras_Lon_Range", "\"Longitude\" BETWEEN -180 AND 180");
        });

        b.HasKey(x => x.Id);

        // Alinha com DataAnnotations (ou atualiza as annotations — escolhe um único sítio)
        b.Property(x => x.Nome).IsRequired().HasMaxLength(160);
        b.Property(x => x.Descricao).IsRequired().HasMaxLength(500);
        b.Property(x => x.Morada).IsRequired().HasMaxLength(200);

        b.Property(x => x.Latitude).HasColumnType("double precision");
        b.Property(x => x.Longitude).HasColumnType("double precision");

        b.Property(x => x.Ativa).IsRequired().HasDefaultValue(true);

        b.HasOne(x => x.Cliente)
         .WithMany(c => c.Obras)
         .HasForeignKey(x => x.ClienteId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.ClienteId, x.Nome }).IsUnique(); 
        b.HasIndex(x => x.Ativa);
    }
}