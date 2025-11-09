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

        // aplica migrações pendentes
        await db.Database.MigrateAsync();

        // 1) CLIENTES — cria exatamente 5
        if (!await db.Clientes.AnyAsync())
        {
            var clientes = Enumerable.Range(1, 5).Select(i => new Cliente
            {
                Nome = $"Cliente {i:D2}",
                NIF = $"{200000000 + i:D9}",              // 9 dígitos únicos
                Morada = $"Rua {i}, Viana do Castelo",
                Email = $"cliente{i}@example.com",
                Telefone = $"91{(1000000 + i):D7}".Substring(0, 9) // 9 dígitos
            }).ToList();

            db.Clientes.AddRange(clientes);
            await db.SaveChangesAsync();
        }

        // 2) MATERIAIS — cria exatamente 1
        if (!await db.Materiais.AnyAsync())
        {
            db.Materiais.Add(new Material
            {
                Nome = "Cimento 25kg",
                Descricao = "Saco 25kg",
                StockDisponivel = 100
            });
            await db.SaveChangesAsync();
        }

        // 3) OBRAS — cria exatamente 1 (associada ao 1.º cliente)
        if (!await db.Obras.AnyAsync())
        {
            var cliente = await db.Clientes.OrderBy(c => c.Id).FirstAsync();

            db.Obras.Add(new Obra
            {
                Nome = "Moradia Viana",
                Descricao = "Construção de moradia unifamiliar",
                ClienteId = cliente.Id,
                Morada = "Viana do Castelo",
                Latitude = 41.693,   // double (sem 'm')
                Longitude = -8.834,  // double (sem 'm')
                Ativa = true
            });
            await db.SaveChangesAsync();
        }

        // 4) PAGAMENTOS — cria exatamente 1 (ligado à 1.ª obra)
        if (!await db.Pagamentos.AnyAsync())
        {
            var obra = await db.Obras.OrderBy(o => o.Id).FirstAsync();

            db.Pagamentos.Add(new Pagamento
            {
                ObraId = obra.Id,
                Nome = "Sinal",
                Valor = 500m,
                DataHora = DateTime.UtcNow.AddDays(-5)
            });
            await db.SaveChangesAsync();
        }

        // 5) MÃO-DE-OBRA — cria exatamente 1 (ligado à 1.ª obra)
        if (!await db.MaosDeObra.AnyAsync())
        {
            var obra = await db.Obras.OrderBy(o => o.Id).FirstAsync();

            db.MaosDeObra.Add(new MaoDeObra
            {
                ObraId = obra.Id,
                Nome = "Pedreiro",
                HorasTrabalhadas = 8m,
                DataHora = DateTime.UtcNow.AddDays(-1)
            });
            await db.SaveChangesAsync();
        }

        // 6) MOVIMENTOS DE MATERIAL — cria exatamente 1 (Entrada)
        if (!await db.MovimentosMaterial.AnyAsync())
        {
            var obra = await db.Obras.OrderBy(o => o.Id).FirstAsync();
            var mat = await db.Materiais.OrderBy(m => m.Id).FirstAsync();

            db.MovimentosMaterial.Add(new MovimentoMaterial
            {
                ObraId = obra.Id,
                MaterialId = mat.Id,
                Quantidade = 50,
                Operacao = OperacaoStock.Entrada,
                DataHora = DateTime.UtcNow.AddDays(-3)
            });

            // Nota: o StockDisponivel não é recalculado aqui; o ajuste é feito pelos controllers em operações reais.
            await db.SaveChangesAsync();
        }
    }
}