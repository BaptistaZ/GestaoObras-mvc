using GestaoObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoObras.Data.Configurations;

public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> b)
    {
        b.ToTable("Pagamentos", tb =>
        {
            // Valor >= 0
            tb.HasCheckConstraint("CK_Pagamentos_Valor_Pos", "\"Valor\" >= 0");
        });

        b.HasKey(x => x.Id);

        b.Property(x => x.Nome).IsRequired().HasMaxLength(120);
        b.Property(x => x.Valor).HasPrecision(12, 2);
        b.Property(x => x.DataHora).IsRequired().HasDefaultValueSql("now() at time zone 'utc'");

        b.HasOne(x => x.Obra)
         .WithMany(o => o.Pagamentos)
         .HasForeignKey(x => x.ObraId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.DataHora);
        b.HasIndex(x => new { x.ObraId, x.DataHora });
    }
}