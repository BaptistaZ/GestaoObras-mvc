using GestaoObras.Data.Context;
using GestaoObras.Domain.Entities;
using GestaoObras.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoObras.Web.Seed;

public static class DataSeeder
{
    /// Aplica migrações e semeia dados mínimos, de forma idempotente.
    public static async Task SeedAsync(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ObrasDbContext>();

        // 0) Migrations pendentes
        await db.Database.MigrateAsync();

        // Usamos uma transação: se algo falhar, nada fica a meio.
        await using var tx = await db.Database.BeginTransactionAsync();

        // 1) CLIENTES — exatamente 5
        if (!await db.Clientes.AnyAsync())
        {
            var clientes = Enumerable.Range(1, 5).Select(i => new Cliente
            {
                Nome     = $"Cliente {i:D2}",
                NIF      = (200_000_000 + i).ToString("D9"),              // 9 dígitos
                Morada   = $"Rua {i}, Viana do Castelo",
                Email    = $"cliente{i}@example.com",
                Telefone = $"91{i:0000000}"                               // 9 dígitos começando por 91
            }).ToList();

            db.Clientes.AddRange(clientes);
            await db.SaveChangesAsync();
        }

        // 2) MATERIAIS — exatamente 1
        if (!await db.Materiais.AnyAsync())
        {
            db.Materiais.Add(new Material
            {
                Nome             = "Cimento 25kg",
                Descricao        = "Saco 25kg",
                StockDisponivel  = 100
            });

            await db.SaveChangesAsync();
        }

        // 3) OBRAS — exatamente 1 (associada ao 1.º cliente)
        if (!await db.Obras.AnyAsync())
        {
            var cliente = await db.Clientes.OrderBy(c => c.Id).FirstAsync();

            db.Obras.Add(new Obra
            {
                Nome       = "Moradia Viana",
                Descricao  = "Construção de moradia unifamiliar",
                ClienteId  = cliente.Id,
                Morada     = "Viana do Castelo",
                Latitude   = 41.693,
                Longitude  = -8.834,
                Ativa      = true
            });

            await db.SaveChangesAsync();
        }

        // 4) PAGAMENTOS — exatamente 1 (ligado à 1.ª obra)
        if (!await db.Pagamentos.AnyAsync())
        {
            var obra = await db.Obras.OrderBy(o => o.Id).FirstAsync();

            db.Pagamentos.Add(new Pagamento
            {
                ObraId   = obra.Id,
                Nome     = "Sinal",
                Valor    = 500m,
                DataHora = DateTime.UtcNow.AddDays(-5)
            });

            await db.SaveChangesAsync();
        }

        // 5) MÃO-DE-OBRA — exatamente 1 (ligado à 1.ª obra)
        if (!await db.MaosDeObra.AnyAsync())
        {
            var obra = await db.Obras.OrderBy(o => o.Id).FirstAsync();

            db.MaosDeObra.Add(new MaoDeObra
            {
                ObraId           = obra.Id,
                Nome             = "Pedreiro",
                HorasTrabalhadas = 8m,
                DataHora         = DateTime.UtcNow.AddDays(-1)
            });

            await db.SaveChangesAsync();
        }

        // 6) MOVIMENTOS DE MATERIAL — exatamente 1 (Entrada) + aplica stock
        if (!await db.MovimentosMaterial.AnyAsync())
        {
            var obra = await db.Obras.OrderBy(o => o.Id).FirstAsync();
            var mat  = await db.Materiais.OrderBy(m => m.Id).FirstAsync();

            await AdicionarMovimentoEAplicarStockAsync(
                db,
                new MovimentoMaterial
                {
                    ObraId    = obra.Id,
                    MaterialId= mat.Id,
                    Quantidade= 50,
                    Operacao  = OperacaoStock.Entrada,
                    DataHora  = DateTime.UtcNow.AddDays(-3)
                });
        }

        await tx.CommitAsync();
    }

    /// Cria um movimento e ajusta o stock do material na mesma transação do DbContext.
    private static async Task AdicionarMovimentoEAplicarStockAsync(ObrasDbContext db, MovimentoMaterial mov)
    {
        // Carrega o material e aplica o delta
        var mat = await db.Materiais.FirstAsync(m => m.Id == mov.MaterialId);

        var delta = mov.Operacao == OperacaoStock.Entrada
            ? +mov.Quantidade
            : -mov.Quantidade;

        var novoStock = mat.StockDisponivel + delta;
        if (novoStock < 0)
            throw new InvalidOperationException("Stock insuficiente ao semear dados.");

        mat.StockDisponivel = novoStock;

        db.MovimentosMaterial.Add(mov);
        await db.SaveChangesAsync();
    }
}