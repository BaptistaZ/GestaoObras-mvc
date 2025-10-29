using GestaoObras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoObras.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> b)
    {
        b.ToTable("Clientes");
        b.HasKey(x => x.Id);

        b.Property(x => x.Nome).IsRequired().HasMaxLength(120);
        b.Property(x => x.NIF).IsRequired().HasMaxLength(9);
        b.Property(x => x.Morada).IsRequired().HasMaxLength(200);
        b.Property(x => x.Email).IsRequired().HasMaxLength(150);
        b.Property(x => x.Telefone).IsRequired().HasMaxLength(20);

        b.HasIndex(x => x.NIF).IsUnique();
        b.HasIndex(x => x.Email);
    }
}