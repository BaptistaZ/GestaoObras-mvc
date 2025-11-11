using GestaoObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoObras.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> b)
    {
        b.ToTable("Clientes", tb =>
        {
            // NIF: 9 dígitos
            tb.HasCheckConstraint("CK_Clientes_NIF_9_DIGITOS", "\"NIF\" ~ '^[0-9]{9}$'");
        });

        b.HasKey(x => x.Id);

        b.Property(x => x.Nome).IsRequired().HasMaxLength(120);
        b.Property(x => x.NIF).IsRequired().HasMaxLength(9);

        // Contactos podem ser opcionais no domínio
        b.Property(x => x.Email).HasMaxLength(150);
        b.Property(x => x.Telefone).HasMaxLength(20);
        b.Property(x => x.Morada).IsRequired().HasMaxLength(200);

        // Regras e índices
        b.HasIndex(x => x.NIF).IsUnique();
        b.HasIndex(x => x.Email);
        b.HasIndex(x => x.Nome);
    }
}