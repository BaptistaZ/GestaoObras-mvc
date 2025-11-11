using GestaoObras.Data.Context;
using GestaoObras.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoObras.Web.Controllers;

public class MateriaisController : Controller
{
    private readonly ObrasDbContext _context;

    public MateriaisController(ObrasDbContext context) => _context = context;

    // GET: Materiais
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = _context.Materiais
            .AsNoTracking()
            .OrderBy(m => m.Nome)
            .ThenBy(m => m.Id);

        var total = await q.CountAsync(ct);
        var items = await q.Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToListAsync(ct);

        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Total = total;

        return View(items);
    }

    // GET: Materiais/Details/5
    public async Task<IActionResult> Details(int? id, CancellationToken ct = default)
    {
        if (id == null) return NotFound();

        var material = await _context.Materiais
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, ct);

        if (material == null) return NotFound();
        return View(material);
    }

    // GET: Materiais/Create
    public IActionResult Create() => View();

    // POST: Materiais/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Id,Nome,Descricao,StockDisponivel")] Material material,
        CancellationToken ct)
    {
        // Normalizações
        material.Nome = (material.Nome ?? string.Empty).Trim();
        material.Descricao = material.Descricao?.Trim();

        // Unicidade (case-insensitive) no PostgreSQL
        if (await _context.Materiais.AnyAsync(
                m => EF.Functions.ILike(m.Nome, material.Nome), ct))
        {
            ModelState.AddModelError(nameof(material.Nome), "Já existe um material com este nome.");
        }

        if (!ModelState.IsValid) return View(material);

        try
        {
            _context.Add(material);
            await _context.SaveChangesAsync(ct);
            TempData["Success"] = "Material criado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível guardar. Verifique duplicados e tente novamente.");
            return View(material);
        }
    }

    // GET: Materiais/Edit/5
    public async Task<IActionResult> Edit(int? id, CancellationToken ct = default)
    {
        if (id == null) return NotFound();

        var material = await _context.Materiais.FindAsync(new object?[] { id }, ct);
        if (material == null) return NotFound();

        return View(material);
    }

    // POST: Materiais/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,Nome,Descricao,StockDisponivel")] Material material,
        CancellationToken ct)
    {
        if (id != material.Id) return NotFound();

        // Normalizações
        material.Nome = (material.Nome ?? string.Empty).Trim();
        material.Descricao = material.Descricao?.Trim();

        // Unicidade (case-insensitive), excluindo o próprio
        if (await _context.Materiais.AnyAsync(
                m => m.Id != id && EF.Functions.ILike(m.Nome, material.Nome), ct))
        {
            ModelState.AddModelError(nameof(material.Nome), "Já existe um material com este nome.");
        }

        if (!ModelState.IsValid) return View(material);

        try
        {
            _context.Update(material);
            await _context.SaveChangesAsync(ct);
            TempData["Success"] = "Material atualizado.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.Materiais.AsNoTracking().AnyAsync(m => m.Id == id, ct);
            if (!exists) return NotFound();
            throw;
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível guardar. Verifique duplicados e tente novamente.");
            return View(material);
        }
    }

    // GET: Materiais/Delete/5
    public async Task<IActionResult> Delete(int? id, CancellationToken ct = default)
    {
        if (id == null) return NotFound();

        var material = await _context.Materiais
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, ct);

        if (material == null) return NotFound();
        return View(material);
    }

    // POST: Materiais/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try
        {
            var material = await _context.Materiais.FindAsync(new object?[] { id }, ct);
            if (material != null)
            {
                _context.Materiais.Remove(material);
                await _context.SaveChangesAsync(ct);
                TempData["Success"] = "Material removido.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            // FK Restrict: não permite apagar materiais com movimentos
            TempData["Error"] = "Não é possível apagar este material: existem movimentos registados.";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}