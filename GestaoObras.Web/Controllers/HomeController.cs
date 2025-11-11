using System.Diagnostics;
using GestaoObras.Data.Context;
using GestaoObras.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoObras.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ObrasDbContext _context;

    private const int UltimosMovimentosLimit = 8;

    public HomeController(ILogger<HomeController> logger, ObrasDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        // Usar limites em UTC porque persistimos DataHora em UTC
        var inicioHojeUtc = DateTime.UtcNow.Date;
        var fimHojeUtc    = inicioHojeUtc.AddDays(1);

        // === Consultas em sequência (DbContext não é thread-safe) ===
        var obrasAtivas    = await _context.Obras
            .AsNoTracking()
            .CountAsync(o => o.Ativa, ct);

        var clientesTotal  = await _context.Clientes
            .AsNoTracking()
            .CountAsync(ct);

        var materiaisTotal = await _context.Materiais
            .AsNoTracking()
            .CountAsync(ct);

        var movimentosHoje = await _context.MovimentosMaterial
            .AsNoTracking()
            .CountAsync(m => m.DataHora >= inicioHojeUtc && m.DataHora < fimHojeUtc, ct);

        var ultimosMovs = await _context.MovimentosMaterial
            .AsNoTracking()
            .OrderByDescending(m => m.DataHora)
            .Take(UltimosMovimentosLimit)
            .Select(m => new UltimoMovimentoItem
            {
                Data       = m.DataHora,           // está em UTC; a view faz ToLocalTime()
                Obra       = m.Obra!.Nome,         // navegação traduzida pelo EF
                Material   = m.Material!.Nome,
                Operacao   = m.Operacao.ToString(),
                Quantidade = m.Quantidade
            })
            .ToListAsync(ct);

        var vm = new DashboardVM
        {
            ObrasAtivas       = obrasAtivas,
            ClientesTotal     = clientesTotal,
            MateriaisTotal    = materiaisTotal,
            MovimentosHoje    = movimentosHoje,
            UltimosMovimentos = ultimosMovs
        };

        return View(vm);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}