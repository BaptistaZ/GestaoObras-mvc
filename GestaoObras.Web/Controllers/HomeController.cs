using System.Diagnostics;
using System.IO;
using ClosedXML.Excel;
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

    // ======================== DASHBOARD (VIEW NORMAL) ========================
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var vm = await BuildDashboardViewModel(ct);
        return View(vm);
    }

    // ======================== EXPORTAR TUDO PARA EXCEL ======================
    // Clientes, Obras, Materiais e Movimentos (cada um numa folha)
    public async Task<IActionResult> ExportTudo(CancellationToken ct)
    {
        using var wb = new XLWorkbook();

        // ------------------------ CLIENTES -----------------------------------
        var wsClientes = wb.Worksheets.Add("Clientes");
        var row = 1;

        wsClientes.Cell(row, 1).Value = "Id";
        wsClientes.Cell(row, 2).Value = "Nome";
        wsClientes.Cell(row, 3).Value = "NIF";
        wsClientes.Cell(row, 4).Value = "Morada";
        wsClientes.Cell(row, 5).Value = "Email";
        wsClientes.Cell(row, 6).Value = "Telefone";
        row++;

        var clientes = await _context.Clientes
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .ToListAsync(ct);

        foreach (var c in clientes)
        {
            wsClientes.Cell(row, 1).Value = c.Id;
            wsClientes.Cell(row, 2).Value = c.Nome;
            wsClientes.Cell(row, 3).Value = c.NIF;        
            wsClientes.Cell(row, 4).Value = c.Morada;
            wsClientes.Cell(row, 5).Value = c.Email;
            wsClientes.Cell(row, 6).Value = c.Telefone;
            row++;
        }

        wsClientes.Columns().AdjustToContents();

        // -------------------------- OBRAS ------------------------------------
        var wsObras = wb.Worksheets.Add("Obras");
        row = 1;

        wsObras.Cell(row, 1).Value = "Id";
        wsObras.Cell(row, 2).Value = "Nome";
        wsObras.Cell(row, 3).Value = "Descrição";
        wsObras.Cell(row, 4).Value = "Cliente";
        wsObras.Cell(row, 5).Value = "Morada";
        wsObras.Cell(row, 6).Value = "Latitude";
        wsObras.Cell(row, 7).Value = "Longitude";
        wsObras.Cell(row, 8).Value = "Ativa";
        row++;

        var obras = await _context.Obras
            .AsNoTracking()
            .Include(o => o.Cliente)
            .OrderBy(o => o.Nome)
            .ToListAsync(ct);

        foreach (var o in obras)
        {
            wsObras.Cell(row, 1).Value = o.Id;
            wsObras.Cell(row, 2).Value = o.Nome;
            wsObras.Cell(row, 3).Value = o.Descricao;          
            wsObras.Cell(row, 4).Value = o.Cliente?.Nome;
            wsObras.Cell(row, 5).Value = o.Morada;
            wsObras.Cell(row, 6).Value = o.Latitude;
            wsObras.Cell(row, 7).Value = o.Longitude;
            wsObras.Cell(row, 8).Value = o.Ativa;
            row++;
        }

        wsObras.Columns().AdjustToContents();

        // ------------------------ MATERIAIS ---------------------------------
        var wsMateriais = wb.Worksheets.Add("Materiais");
        row = 1;

        wsMateriais.Cell(row, 1).Value = "Id";
        wsMateriais.Cell(row, 2).Value = "Nome";
        wsMateriais.Cell(row, 3).Value = "Descrição";
        wsMateriais.Cell(row, 4).Value = "Stock disponível";
        row++;

        var materiais = await _context.Materiais
            .AsNoTracking()
            .OrderBy(m => m.Nome)
            .ToListAsync(ct);

        foreach (var m in materiais)
        {
            wsMateriais.Cell(row, 1).Value = m.Id;
            wsMateriais.Cell(row, 2).Value = m.Nome;
            wsMateriais.Cell(row, 3).Value = m.Descricao;
            wsMateriais.Cell(row, 4).Value = m.StockDisponivel;
            row++;
        }

        wsMateriais.Columns().AdjustToContents();

        // ---------------------- MOVIMENTOS MATERIAL -------------------------
        var wsMovs = wb.Worksheets.Add("Movimentos");
        row = 1;

        wsMovs.Cell(row, 1).Value = "Id";
        wsMovs.Cell(row, 2).Value = "Data/Hora";
        wsMovs.Cell(row, 3).Value = "Obra";
        wsMovs.Cell(row, 4).Value = "Material";
        wsMovs.Cell(row, 5).Value = "Operação";
        wsMovs.Cell(row, 6).Value = "Quantidade";
        row++;

        var movimentos = await _context.MovimentosMaterial
            .AsNoTracking()
            .Include(m => m.Obra)
            .Include(m => m.Material)
            .OrderByDescending(m => m.DataHora)
            .ToListAsync(ct);

        foreach (var mv in movimentos)
        {
            wsMovs.Cell(row, 1).Value = mv.Id;
            wsMovs.Cell(row, 2).Value = mv.DataHora.ToLocalTime();
            wsMovs.Cell(row, 2).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
            wsMovs.Cell(row, 3).Value = mv.Obra?.Nome;
            wsMovs.Cell(row, 4).Value = mv.Material?.Nome;
            wsMovs.Cell(row, 5).Value = mv.Operacao.ToString();
            wsMovs.Cell(row, 6).Value = mv.Quantidade;
            row++;
        }

        wsMovs.Columns().AdjustToContents();

        // ---------------------- devolver ficheiro ---------------------------
        using var stream = new MemoryStream();
        wb.SaveAs(stream);
        var fileName = $"gestao-obras-dados-{DateTime.UtcNow:yyyyMMdd-HHmm}.xlsx";

        return File(
            fileContents: stream.ToArray(),
            contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileDownloadName: fileName
        );
    }

    // ======================== VIEWMODEL DO DASHBOARD ========================
    private async Task<DashboardVM> BuildDashboardViewModel(CancellationToken ct)
    {
        var inicioHojeUtc = DateTime.UtcNow.Date;
        var fimHojeUtc    = inicioHojeUtc.AddDays(1);

        var obrasAtivas = await _context.Obras
            .AsNoTracking()
            .CountAsync(o => o.Ativa, ct);

        var clientesTotal = await _context.Clientes
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
            .Include(m => m.Obra)
            .Include(m => m.Material)
            .OrderByDescending(m => m.DataHora)
            .Take(UltimosMovimentosLimit)
            .Select(m => new UltimoMovimentoItem
            {
                Data       = m.DataHora,
                Obra       = m.Obra != null ? m.Obra.Nome : "(sem obra)",
                Material   = m.Material != null ? m.Material.Nome : "(sem material)",
                Operacao   = m.Operacao.ToString(),
                Quantidade = m.Quantidade
            })
            .ToListAsync(ct);

        var obrasResumo = await _context.Obras
            .AsNoTracking()
            .Where(o => o.Ativa)
            .Select(o => new ObraResumoItem
            {
                Id = o.Id,
                Nome = o.Nome,
                Movimentos = o.MovimentosMaterial.Count,
                MateriaisDistintos = o.MovimentosMaterial
                    .Select(m => m.MaterialId)
                    .Distinct()
                    .Count()
            })
            .OrderByDescending(o => o.Movimentos)
            .ThenBy(o => o.Nome)
            .ToListAsync(ct);

        return new DashboardVM
        {
            ObrasAtivas       = obrasAtivas,
            ClientesTotal     = clientesTotal,
            MateriaisTotal    = materiaisTotal,
            MovimentosHoje    = movimentosHoje,
            UltimosMovimentos = ultimosMovs,
            ObrasResumo       = obrasResumo
        };
    }

    // ======================== RESTO DOS ACTIONS =============================
    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}