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

        await db.Database.MigrateAsync();

        // 1) CLIENTES (cria 60 com todos os campos obrigatórios)
        if (!await db.Clientes.AnyAsync())
        {
            var clientes = Enumerable.Range(1, 60).Select(i => new Cliente
            {
                Nome = $"Cliente {i:D2}",
                NIF = $"{200000000 + i:D9}",        // 9 dígitos
                Morada = $"Rua {i}, Viana do Castelo",
                Email = $"cliente{i}@example.com",
                Telefone = $"91{(1000000 + i):D7}".Substring(0, 9) // 9 dígitos
            }).ToList();

            db.Clientes.AddRange(clientes);
            await db.SaveChangesAsync();
        }

        // 2) MATERIAIS
        if (!await db.Materiais.AnyAsync())
        {
            db.Materiais.AddRange(
                new Material { Nome = "Cimento 25kg", Descricao = "Saco 25kg", StockDisponivel = 100 },
                new Material { Nome = "Tijolo 11", Descricao = "Furado", StockDisponivel = 500 },
                new Material { Nome = "Aço 8mm", Descricao = "Vareta", StockDisponivel = 200 }
            );
            await db.SaveChangesAsync();
        }

        // 3) OBRAS (usa sempre um cliente existente)
        if (!await db.Obras.AnyAsync())
        {
            var cliente = await db.Clientes.OrderBy(c => c.Id).FirstAsync(); // agora existe de certeza

            db.Obras.AddRange(
                new Obra
                {
                    Nome = "Moradia Viana",
                    Descricao = "Construção de moradia unifamiliar",
                    ClienteId = cliente.Id,
                    Morada = "Viana do Castelo",
                    Latitude = 41.693, Longitude = -8.834, Ativa = true
                },
                new Obra
                {
                    Nome = "Reabilitação Centro",
                    Descricao = "Reabilitação de edifício no centro",
                    ClienteId = cliente.Id,
                    Morada = "Braga",
                    Latitude = 41.545, Longitude = -8.426, Ativa = true
                },
                new Obra
                {
                    Nome = "Armazém Industrial",
                    Descricao = "Novo armazém",
                    ClienteId = cliente.Id,
                    Morada = "Porto",
                    Latitude = 41.149, Longitude = -8.610, Ativa = false
                }
            );
            await db.SaveChangesAsync();
        }

        // 4) PAGAMENTOS (liga à 1ª obra existente)
        if (!await db.Pagamentos.AnyAsync())
        {
            var obra = await db.Obras.OrderBy(o => o.Id).FirstAsync();

            db.Pagamentos.AddRange(
                new Pagamento { ObraId = obra.Id, Nome = "Sinal", Valor = 500m, DataHora = DateTime.UtcNow.AddDays(-10) },
                new Pagamento { ObraId = obra.Id, Nome = "1ª Fase", Valor = 1500m, DataHora = DateTime.UtcNow.AddDays(-5) },
                new Pagamento { ObraId = obra.Id, Nome = "2ª Fase", Valor = 2000m, DataHora = DateTime.UtcNow.AddDays(-2) }
            );
            await db.SaveChangesAsync();
        }

        // 5) MÃO-DE-OBRA
        if (!await db.MaosDeObra.AnyAsync())
        {
            var obra = await db.Obras.OrderBy(o => o.Id).FirstAsync();

            db.MaosDeObra.AddRange(
                new MaoDeObra { ObraId = obra.Id, Nome = "Pedreiro", HorasTrabalhadas = 8m, DataHora = DateTime.UtcNow.AddDays(-1) },
                new MaoDeObra { ObraId = obra.Id, Nome = "Eletricista", HorasTrabalhadas = 6.5m, DataHora = DateTime.UtcNow.AddDays(-1) }
            );
            await db.SaveChangesAsync();
        }

        // 6) MOVIMENTOS DE MATERIAL (Entrada/Saída)
        if (!await db.MovimentosMaterial.AnyAsync())
        {
            var obra = await db.Obras.OrderBy(o => o.Id).FirstAsync();
            var mat = await db.Materiais.OrderBy(m => m.Id).FirstAsync();

            db.MovimentosMaterial.AddRange(
                new MovimentoMaterial { ObraId = obra.Id, MaterialId = mat.Id, Quantidade = 50, Operacao = OperacaoStock.Entrada, DataHora = DateTime.UtcNow.AddDays(-5) },
                new MovimentoMaterial { ObraId = obra.Id, MaterialId = mat.Id, Quantidade = 10, Operacao = OperacaoStock.Saida, DataHora = DateTime.UtcNow.AddDays(-2) }
            );
            await db.SaveChangesAsync();
        }
    }
}