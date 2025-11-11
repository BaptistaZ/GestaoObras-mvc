using GestaoObras.Data.Context;
using GestaoObras.Domain.Entities;
using GestaoObras.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestaoObras.Web.Controllers;

public class MovimentosMaterialController : Controller
{
    private readonly ObrasDbContext _context;
    public MovimentosMaterialController(ObrasDbContext context) => _context = context;

    // Helpers
    private static DateTime ToUtc(DateTime dt)
    {
        if (dt == default) return DateTime.UtcNow;
        if (dt.Kind == DateTimeKind.Unspecified) dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
        return dt.ToUniversalTime();
    }

    private static int Delta(MovimentoMaterial m) =>
        m.Operacao == OperacaoStock.Entrada ? +m.Quantidade : -m.Quantidade;

    private static bool TryApply(Material mat, int delta)
    {
        var novo = mat.StockDisponivel + delta;
        if (novo < 0) return false;
        mat.StockDisponivel = novo;
        return true;
    }

    // LIST
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var q = _context.MovimentosMaterial
            .AsNoTracking()
            .Include(m => m.Material)
            .Include(m => m.Obra)
            .OrderByDescending(m => m.DataHora)
            .ThenByDescending(m => m.Id);

        var data = await q.ToListAsync(ct);
        return View(data);
    }

    // DETAILS
    public async Task<IActionResult> Details(int? id, CancellationToken ct)
    {
        if (id == null) return NotFound();

        var mov = await _context.MovimentosMaterial
            .AsNoTracking()
            .Include(m => m.Material)
            .Include(m => m.Obra)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

        if (mov == null) return NotFound();
        return View(mov);
    }

    // GET CREATE
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        ViewData["MaterialId"] = new SelectList(
            await _context.Materiais.AsNoTracking().OrderBy(m => m.Nome).ToListAsync(ct), "Id", "Nome");

        ViewData["ObraId"] = new SelectList(
            await _context.Obras.AsNoTracking().OrderBy(o => o.Nome).ToListAsync(ct), "Id", "Nome");

        return View();
    }

    // POST CREATE (aplica stock)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Id,ObraId,MaterialId,Quantidade,DataHora,Operacao")] MovimentoMaterial mov,
        string? returnUrl,
        CancellationToken ct)
    {
        mov.DataHora = ToUtc(mov.DataHora);

        var material = await _context.Materiais.FindAsync(new object?[] { mov.MaterialId }, ct);
        if (material == null)
            ModelState.AddModelError(nameof(mov.MaterialId), "Material inválido.");

        var obraExists = await _context.Obras.AsNoTracking().AnyAsync(o => o.Id == mov.ObraId, ct);
        if (!obraExists)
            ModelState.AddModelError(nameof(mov.ObraId), "Obra inválida.");

        if (!ModelState.IsValid)
        {
            await LoadSelectsAsync(mov.MaterialId, mov.ObraId, ct);
            return View(mov);
        }

        var delta = Delta(mov);

        await using var tx = await _context.Database.BeginTransactionAsync(ct);

        if (!TryApply(material!, delta))
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                TempData["Error"] = "Stock insuficiente para esta saída.";
                return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError(nameof(mov.Quantidade), "Stock insuficiente para esta saída.");
            await LoadSelectsAsync(mov.MaterialId, mov.ObraId, ct);
            return View(mov);
        }

        _context.MovimentosMaterial.Add(mov);
        await _context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Details", "Obras", new { id = mov.ObraId, tab = "mov" });
    }

    // GET EDIT
    public async Task<IActionResult> Edit(int? id, CancellationToken ct)
    {
        if (id == null) return NotFound();

        var mov = await _context.MovimentosMaterial.FindAsync(new object?[] { id }, ct);
        if (mov == null) return NotFound();

        await LoadSelectsAsync(mov.MaterialId, mov.ObraId, ct);
        return View(mov);
    }

    // POST EDIT (reverte antigo e aplica novo)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,ObraId,MaterialId,Quantidade,DataHora,Operacao")] MovimentoMaterial updated,
        string? returnUrl,
        CancellationToken ct)
    {
        if (id != updated.Id) return NotFound();
        updated.DataHora = ToUtc(updated.DataHora);

        var original = await _context.MovimentosMaterial
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, ct);

        if (original == null) return NotFound();

        var matOld = await _context.Materiais.FindAsync(new object?[] { original.MaterialId }, ct);
        var matNew = original.MaterialId == updated.MaterialId
            ? matOld
            : await _context.Materiais.FindAsync(new object?[] { updated.MaterialId }, ct);

        if (matOld == null || matNew == null)
        {
            ModelState.AddModelError(string.Empty, "Material inválido.");
            await LoadSelectsAsync(updated.MaterialId, updated.ObraId, ct);
            return View(updated);
        }

        await using var tx = await _context.Database.BeginTransactionAsync(ct);

        // 1) Reverter efeito antigo
        var oldDelta = Delta(original);
        if (!TryApply(matOld, -oldDelta))
        {
            await tx.RollbackAsync(ct);
            ModelState.AddModelError(string.Empty, "Falha ao reverter stock anterior.");
            await LoadSelectsAsync(updated.MaterialId, updated.ObraId, ct);
            return View(updated);
        }

        // 2) Aplicar novo
        var newDelta = Delta(updated);
        if (!TryApply(matNew, newDelta))
        {
            // Repor estado anterior
            TryApply(matOld, oldDelta);
            await tx.RollbackAsync(ct);

            ModelState.AddModelError(nameof(updated.Quantidade), "Stock insuficiente para esta saída.");
            await LoadSelectsAsync(updated.MaterialId, updated.ObraId, ct);
            return View(updated);
        }

        _context.Update(updated);
        await _context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    // GET DELETE
    public async Task<IActionResult> Delete(int? id, CancellationToken ct)
    {
        if (id == null) return NotFound();

        var mov = await _context.MovimentosMaterial
            .AsNoTracking()
            .Include(m => m.Material)
            .Include(m => m.Obra)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

        if (mov == null) return NotFound();
        return View(mov);
    }

    // POST DELETE (reverte stock)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl, CancellationToken ct)
    {
        var mov = await _context.MovimentosMaterial.FindAsync(new object?[] { id }, ct);
        if (mov != null)
        {
            var mat = await _context.Materiais.FindAsync(new object?[] { mov.MaterialId }, ct);
            if (mat != null)
            {
                await using var tx = await _context.Database.BeginTransactionAsync(ct);

                // Reverter o efeito do movimento eliminado
                TryApply(mat, -Delta(mov));
                _context.MovimentosMaterial.Remove(mov);

                await _context.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                TempData["Success"] = "Movimento removido e stock ajustado.";
            }
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    // Utils
    private async Task LoadSelectsAsync(int? materialId, int? obraId, CancellationToken ct)
    {
        ViewData["MaterialId"] = new SelectList(
            await _context.Materiais.AsNoTracking().OrderBy(m => m.Nome).ToListAsync(ct),
            "Id", "Nome", materialId);

        ViewData["ObraId"] = new SelectList(
            await _context.Obras.AsNoTracking().OrderBy(o => o.Nome).ToListAsync(ct),
            "Id", "Nome", obraId);
    }
}