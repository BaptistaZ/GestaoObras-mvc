using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestaoObras.Data.Context;
using GestaoObras.Domain.Entities;
using GestaoObras.Domain.Enums; // garante o enum OperacaoStock

namespace GestaoObras.Web.Controllers
{
    public class MovimentosMaterialController : Controller
    {
        private readonly ObrasDbContext _context;
        public MovimentosMaterialController(ObrasDbContext context) => _context = context;

        // ---- Helpers ----
        private static DateTime ToUtc(DateTime dt)
        {
            if (dt == default) return DateTime.UtcNow;
            if (dt.Kind == DateTimeKind.Unspecified) dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
            return dt.ToUniversalTime();
        }

        private static int Delta(MovimentoMaterial m) =>
            m.Operacao == OperacaoStock.Entrada ? +m.Quantidade : -m.Quantidade;

        // aplica delta ao material; valida não-negativo
        private static bool TryApply(Material mat, int delta)
        {
            var novo = mat.StockDisponivel + delta;
            if (novo < 0) return false;
            mat.StockDisponivel = novo;
            return true;
        }

        // ---- CRUD “listar” (mantidos) ----
        public async Task<IActionResult> Index()
        {
            var q = _context.MovimentosMaterial.Include(m => m.Material).Include(m => m.Obra);
            return View(await q.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var mov = await _context.MovimentosMaterial
                .Include(m => m.Material).Include(m => m.Obra)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mov == null) return NotFound();
            return View(mov);
        }

        public IActionResult Create()
        {
            ViewData["MaterialId"] = new SelectList(_context.Materiais, "Id", "Nome");
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Descricao");
            return View();
        }

        // ---- CREATE (aplica stock) ----
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,ObraId,MaterialId,Quantidade,DataHora,Operacao")] MovimentoMaterial mov,
            string? returnUrl)
        {
            mov.DataHora = ToUtc(mov.DataHora);

            var material = await _context.Materiais.FindAsync(mov.MaterialId);
            if (material == null)
                ModelState.AddModelError(nameof(mov.MaterialId), "Material inválido.");

            if (!ModelState.IsValid)
            {
                ViewData["MaterialId"] = new SelectList(_context.Materiais, "Id", "Nome", mov.MaterialId);
                ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Descricao", mov.ObraId);
                return View(mov);
            }

            var delta = Delta(mov);

            using var tx = await _context.Database.BeginTransactionAsync();
            if (!TryApply(material!, delta))
            {
                ModelState.AddModelError(nameof(mov.Quantidade), "Stock insuficiente para esta saída.");
                ViewData["MaterialId"] = new SelectList(_context.Materiais, "Id", "Nome", mov.MaterialId);
                ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Descricao", mov.ObraId);
                return View(mov);
            }

            _context.MovimentosMaterial.Add(mov);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            if (!string.IsNullOrWhiteSpace(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction(nameof(Index));
        }

        // ---- EDIT (reverte antigo e aplica novo; suporta troca de material) ----
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var mov = await _context.MovimentosMaterial.FindAsync(id);
            if (mov == null) return NotFound();

            ViewData["MaterialId"] = new SelectList(_context.Materiais, "Id", "Nome", mov.MaterialId);
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Descricao", mov.ObraId);
            return View(mov);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ObraId,MaterialId,Quantidade,DataHora,Operacao")] MovimentoMaterial updated)
        {
            if (id != updated.Id) return NotFound();
            updated.DataHora = ToUtc(updated.DataHora);

            var original = await _context.MovimentosMaterial.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (original == null) return NotFound();

            var matOld = await _context.Materiais.FindAsync(original.MaterialId);
            var matNew = original.MaterialId == updated.MaterialId
                ? matOld
                : await _context.Materiais.FindAsync(updated.MaterialId);

            if (matOld == null || matNew == null)
            {
                ModelState.AddModelError("", "Material inválido.");
                ViewData["MaterialId"] = new SelectList(_context.Materiais, "Id", "Nome", updated.MaterialId);
                ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Descricao", updated.ObraId);
                return View(updated);
            }

            using var tx = await _context.Database.BeginTransactionAsync();

            // 1) reverter efeito antigo
            var oldDelta = Delta(original);
            if (!TryApply(matOld, -oldDelta))
            {
                // não deve falhar (estamos a “devolver” stock), mas por segurança:
                await tx.RollbackAsync();
                ModelState.AddModelError("", "Falha ao reverter stock antigo.");
                return View(updated);
            }

            // 2) aplicar novo efeito (pode ser noutro material)
            var newDelta = Delta(updated);
            if (!TryApply(matNew, newDelta))
            {
                // volta a aplicar o antigo para não deixar stock incoerente
                TryApply(matOld, oldDelta);
                await tx.RollbackAsync();

                ModelState.AddModelError(nameof(updated.Quantidade), "Stock insuficiente para esta saída.");
                ViewData["MaterialId"] = new SelectList(_context.Materiais, "Id", "Nome", updated.MaterialId);
                ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Descricao", updated.ObraId);
                return View(updated);
            }

            _context.Update(updated);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // ---- DELETE (reverte efeito no stock) ----
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var mov = await _context.MovimentosMaterial
                .Include(m => m.Material).Include(m => m.Obra)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mov == null) return NotFound();
            return View(mov);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mov = await _context.MovimentosMaterial.FindAsync(id);
            if (mov != null)
            {
                var mat = await _context.Materiais.FindAsync(mov.MaterialId);
                if (mat != null)
                {
                    using var tx = await _context.Database.BeginTransactionAsync();
                    // reverter o movimento
                    TryApply(mat, -Delta(mov));
                    _context.MovimentosMaterial.Remove(mov);
                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MovimentoMaterialExists(int id) =>
            _context.MovimentosMaterial.Any(e => e.Id == id);
    }
}