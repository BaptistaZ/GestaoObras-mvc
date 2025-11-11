using GestaoObras.Data.Context;
using GestaoObras.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestaoObras.Web.Controllers;

public class ObrasController : Controller
{
    private readonly ObrasDbContext _context;

    public ObrasController(ObrasDbContext context) => _context = context;

    // GET: Obras
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var obras = await _context.Obras
            .AsNoTracking()
            .Include(o => o.Cliente)
            .OrderBy(o => o.Nome).ThenBy(o => o.Id)
            .ToListAsync(ct);

        return View(obras);
    }

    // GET: Obras/Details/5
    public async Task<IActionResult> Details(int id, string? tab = null, CancellationToken ct = default)
    {
        var obra = await _context.Obras
            .AsNoTracking()
            .Include(o => o.Cliente)
            .Include(o => o.MovimentosMaterial).ThenInclude(m => m.Material)
            .Include(o => o.MaosDeObra)
            .Include(o => o.Pagamentos)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        if (obra == null) return NotFound();

        ViewBag.ActiveTab = tab;

        ViewBag.MaterialId = new SelectList(
            await _context.Materiais
                .AsNoTracking()
                .OrderBy(m => m.Nome).ThenBy(m => m.Id)
                .ToListAsync(ct),
            "Id", "Nome"
        );

        // URL atual para voltar após guardar no Edit
        ViewBag.CurrentUrl = HttpContext?.Request?.Path + HttpContext?.Request?.QueryString;

        return View(obra);
    }

    // GET: Obras/Create
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        ViewData["ClienteId"] = new SelectList(
            await _context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ThenBy(c => c.Id).ToListAsync(ct),
            "Id", "Nome"
        );
        return View();
    }

    // POST: Obras/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    [Bind("Id,Nome,Descricao,ClienteId,Morada,Latitude,Longitude,Ativa")] Obra obra,
    CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var errs = string.Join(" | ",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            TempData["Error"] = string.IsNullOrWhiteSpace(errs)
                ? "Não foi possível criar a obra. Verifique os dados e tente novamente."
                : $"Não foi possível criar a obra: {errs}";

            ViewData["ClienteId"] = new SelectList(
                await _context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ThenBy(c => c.Id).ToListAsync(ct),
                "Id", "Nome", obra.ClienteId
            );
            return View(obra);
        }

        try
        {
            _context.Add(obra);
            await _context.SaveChangesAsync(ct);
            TempData["Success"] = "Obra criada com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Não foi possível criar a obra. Verifique os dados e tente novamente.";
            ViewData["ClienteId"] = new SelectList(
                await _context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ThenBy(c => c.Id).ToListAsync(ct),
                "Id", "Nome", obra.ClienteId
            );
            return View(obra);
        }
    }

    // GET: Obras/Edit/5
    public async Task<IActionResult> Edit(int? id, string? returnUrl = null, CancellationToken ct = default)
    {
        if (id == null) return NotFound();

        var obra = await _context.Obras.FindAsync(new object?[] { id }, ct);
        if (obra == null) return NotFound();

        ViewData["ClienteId"] = new SelectList(
            await _context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ThenBy(c => c.Id).ToListAsync(ct),
            "Id", "Nome", obra.ClienteId
        );
        ViewBag.ReturnUrl = returnUrl;
        return View(obra);
    }

    // POST: Obras/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,Nome,Descricao,ClienteId,Morada,Latitude,Longitude,Ativa")] Obra obra,
        string? returnUrl,
        CancellationToken ct)
    {
        if (id != obra.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["ClienteId"] = new SelectList(
                await _context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ThenBy(c => c.Id).ToListAsync(ct),
                "Id", "Nome", obra.ClienteId
            );
            ViewBag.ReturnUrl = returnUrl;
            return View(obra);
        }

        try
        {
            _context.Update(obra);
            await _context.SaveChangesAsync(ct);
            TempData["Success"] = "Obra atualizada.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Obras.AsNoTracking().AnyAsync(e => e.Id == obra.Id, ct))
                return NotFound();
            throw;
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Não foi possível atualizar a obra. Tente novamente.";
            ViewData["ClienteId"] = new SelectList(
                await _context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ThenBy(c => c.Id).ToListAsync(ct),
                "Id", "Nome", obra.ClienteId
            );
            ViewBag.ReturnUrl = returnUrl;
            return View(obra);
        }

        // Prioridade ao returnUrl (mantém a mesma aba dos detalhes)
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        // Fallback
        return RedirectToAction("Details", new { id = obra.Id });
    }

    // GET: Obras/Delete/5
    public async Task<IActionResult> Delete(int? id, CancellationToken ct)
    {
        if (id == null) return NotFound();

        var obra = await _context.Obras
            .AsNoTracking()
            .Include(o => o.Cliente)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

        if (obra == null) return NotFound();

        return View(obra);
    }

    // POST: Obras/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var obra = await _context.Obras.FindAsync(new object?[] { id }, ct);
        if (obra == null) return RedirectToAction(nameof(Index));

        try
        {
            _context.Obras.Remove(obra);
            await _context.SaveChangesAsync(ct);
            TempData["Success"] = "Obra removida.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Não foi possível apagar a obra (restrições de integridade).";
        }

        return RedirectToAction(nameof(Index));
    }

    private Task<bool> ObraExistsAsync(int id, CancellationToken ct) =>
        _context.Obras.AsNoTracking().AnyAsync(e => e.Id == id, ct);
}