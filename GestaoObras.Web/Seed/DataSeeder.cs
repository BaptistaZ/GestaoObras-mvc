using GestaoObras.Data.Context;
using GestaoObras.Domain.Entities;
using GestaoObras.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestaoObras.Web.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ObrasDbContext>();

        // Garante que a BD/migrations estão aplicadas
        await db.Database.MigrateAsync();

        // --- CLIENTES ---
        if (!await db.Clientes.AnyAsync())
        {
            db.Clientes.AddRange(
                new Cliente { Nome = "Construsul Lda", NIF = "509123456", Morada = "Rua do Norte, 10, Viana",
                              Email = "geral@construsul.pt", Telefone = "+351258000001" },
                new Cliente { Nome = "Habitarte SA",   NIF = "505987321", Morada = "Av. Atlântica, 200, Porto",
                              Email = "contacto@habitarte.pt", Telefone = "+351220100200" },
                new Cliente { Nome = "ObrasMinho",     NIF = "507654987", Morada = "Largo Central, 3, Braga",
                              Email = "info@obrasminho.pt", Telefone = "+351253555111" }
            );
            await db.SaveChangesAsync();
        }

        // --- MATERIAIS ---
        if (!await db.Materiais.AnyAsync())
        {
            db.Materiais.AddRange(
                new Material { Nome = "Cimento 25kg", Descricao = "Saco 25kg", StockDisponivel = 120 },
                new Material { Nome = "Tijolo 11",    Descricao = "Tijolo furado", StockDisponivel = 800 },
                new Material { Nome = "Aço 8mm",      Descricao = "Varão 8mm", StockDisponivel = 300 }
            );
            await db.SaveChangesAsync();
        }

        // --- OBRAS ---
        if (!await db.Obras.AnyAsync())
        {
            var cliente1 = await db.Clientes.OrderBy(c => c.Id).FirstAsync();
            var cliente2 = await db.Clientes.OrderBy(c => c.Id).Skip(1).FirstAsync();
            var cliente3 = await db.Clientes.OrderBy(c => c.Id).Skip(2).FirstAsync();

            db.Obras.AddRange(
                new Obra { Nome = "Moradia Viana", Descricao = "Moradia T3 com garagem",
                           ClienteId = cliente1.Id, Morada = "Rua das Flores, Viana",
                           Latitude = 41.693, Longitude = -8.834, Ativa = true },
                new Obra { Nome = "Reabilitação Centro", Descricao = "Apartamento T2",
                           ClienteId = cliente2.Id, Morada = "Rua do Sol, Porto",
                           Latitude = 41.1579, Longitude = -8.6291, Ativa = true },
                new Obra { Nome = "Armazém Industrial", Descricao = "Pavilhão logístico",
                           ClienteId = cliente3.Id, Morada = "Zona Ind., Braga",
                           Latitude = 41.545, Longitude = -8.426, Ativa = false }
            );
            await db.SaveChangesAsync();
        }

        // --- PAGAMENTOS ---
        if (!await db.Pagamentos.AnyAsync())
        {
            var obras = await db.Obras.OrderBy(o => o.Id).ToListAsync();
            db.Pagamentos.AddRange(
                new Pagamento { ObraId = obras[0].Id, Nome = "Sinalização", Valor = 2500.00m, DataHora = new DateTime(2025,10,01,10,00,00,DateTimeKind.Utc) },
                new Pagamento { ObraId = obras[0].Id, Nome = "1ª Fase",     Valor = 7500.50m, DataHora = new DateTime(2025,10,10,14,30,00,DateTimeKind.Utc) },
                new Pagamento { ObraId = obras[1].Id, Nome = "Sinalização", Valor = 1800.00m, DataHora = new DateTime(2025,10,05,09,15,00,DateTimeKind.Utc) }
            );
            await db.SaveChangesAsync();
        }

        // --- MÃO-DE-OBRA ---
        if (!await db.MaosDeObra.AnyAsync())
        {
            var obras = await db.Obras.OrderBy(o => o.Id).ToListAsync();
            db.MaosDeObra.AddRange(
                new MaoDeObra { ObraId = obras[0].Id, Nome = "Pedreiro",    HorasTrabalhadas = 8.0m, DataHora = new DateTime(2025,10,02,08,00,00,DateTimeKind.Utc) },
                new MaoDeObra { ObraId = obras[0].Id, Nome = "Servente",    HorasTrabalhadas = 7.5m, DataHora = new DateTime(2025,10,02,08,00,00,DateTimeKind.Utc) },
                new MaoDeObra { ObraId = obras[1].Id, Nome = "Canalizador", HorasTrabalhadas = 3.0m, DataHora = new DateTime(2025,10,06,14,00,00,DateTimeKind.Utc) }
            );
            await db.SaveChangesAsync();
        }

        // --- MOVIMENTOS DE MATERIAL ---
        if (!await db.MovimentosMaterial.AnyAsync())
        {
            var obras = await db.Obras.OrderBy(o => o.Id).ToListAsync();
            var mat  = await db.Materiais.OrderBy(m => m.Id).ToListAsync();

            db.MovimentosMaterial.AddRange(
                new MovimentoMaterial { ObraId = obras[0].Id, MaterialId = mat[0].Id, Quantidade = 20,
                                        DataHora = new DateTime(2025,10,03,09,00,00,DateTimeKind.Utc), Operacao = OperacaoStock.Entrada },
                new MovimentoMaterial { ObraId = obras[0].Id, MaterialId = mat[1].Id, Quantidade = 200,
                                        DataHora = new DateTime(2025,10,03,09,30,00,DateTimeKind.Utc), Operacao = OperacaoStock.Entrada },
                new MovimentoMaterial { ObraId = obras[1].Id, MaterialId = mat[2].Id, Quantidade = 35,
                                        DataHora = new DateTime(2025,10,07,10,00,00,DateTimeKind.Utc), Operacao = OperacaoStock.Saida }
            );
            await db.SaveChangesAsync();
        }

        // (Opcional) Gerar mais clientes para testar paginação
        var totalClientes = await db.Clientes.CountAsync();
        if (totalClientes < 60)
        {
            var toAdd = new List<Cliente>();
            for (int i = totalClientes + 1; i <= 60; i++)
            {
                toAdd.Add(new Cliente {
                    Nome = $"Cliente Demo {i}",
                    NIF = (200000000 + i).ToString(),
                    Morada = $"Rua Demo {i}, Cidade",
                    Email = $"cliente{i}@demo.pt",
                    Telefone = $"+3519000{i:0000}"
                });
            }
            await db.Clientes.AddRangeAsync(toAdd);
            await db.SaveChangesAsync();
        }
    }
}