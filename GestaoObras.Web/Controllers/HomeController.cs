using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoObras.Web.Models;
using GestaoObras.Data.Context;

namespace GestaoObras.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ObrasDbContext _context;

    public HomeController(ILogger<HomeController> logger, ObrasDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var hojeUtc = DateTime.UtcNow.Date;

        var vm = new DashboardVM
        {
            ObrasAtivas    = await _context.Obras.CountAsync(o => o.Ativa),
            ClientesTotal  = await _context.Clientes.CountAsync(),
            MateriaisTotal = await _context.Materiais.CountAsync(),
            MovimentosHoje = await _context.MovimentosMaterial.CountAsync(m => m.DataHora >= hojeUtc),

            UltimosMovimentos = await _context.MovimentosMaterial
                .AsNoTracking()
                .Include(m => m.Material)
                .Include(m => m.Obra)
                .OrderByDescending(m => m.DataHora)
                .Take(8)
                .Select(m => new UltimoMovimentoItem {
                    Data = m.DataHora,
                    Obra = m.Obra.Nome,
                    Material = m.Material.Nome,
                    Operacao = m.Operacao.ToString(),
                    Quantidade = m.Quantidade
                })
                .ToListAsync()
        };

        return View(vm);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}