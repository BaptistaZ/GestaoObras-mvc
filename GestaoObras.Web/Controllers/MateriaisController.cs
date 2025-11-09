using System;
using System.Linq;
using System.Threading.Tasks;
using GestaoObras.Data.Context;
using GestaoObras.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoObras.Web.Controllers
{
    public class MateriaisController : Controller
    {
        private readonly ObrasDbContext _context;

        public MateriaisController(ObrasDbContext context)
        {
            _context = context;
        }

        // GET: Materiais
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var q = _context.Materiais.AsNoTracking().OrderBy(m => m.Nome);
            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            ViewBag.Page = page; ViewBag.PageSize = pageSize; ViewBag.Total = total;
            return View(items);
        }

        // GET: Materiais/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var material = await _context.Materiais
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (material == null) return NotFound();

            return View(material);
        }

        // GET: Materiais/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Materiais/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,StockDisponivel")] Material material)
        {
            // valida duplicado por Nome
            if (await _context.Materiais.AnyAsync(m => m.Nome == material.Nome))
                ModelState.AddModelError(nameof(material.Nome), "Já existe um material com este nome.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(material);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Material criado com sucesso.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "Não foi possível guardar. Verifique duplicados e tente novamente.");
                }
            }
            return View(material);
        }

        // GET: Materiais/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var material = await _context.Materiais.FindAsync(id);
            if (material == null) return NotFound();

            return View(material);
        }

        // POST: Materiais/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,StockDisponivel")] Material material)
        {
            if (id != material.Id) return NotFound();

            // valida duplicado por Nome (exclui o próprio)
            if (await _context.Materiais.AnyAsync(m => m.Id != id && m.Nome == material.Nome))
                ModelState.AddModelError(nameof(material.Nome), "Já existe um material com este nome.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(material);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Material atualizado.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialExists(material.Id)) return NotFound(); else throw;
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "Não foi possível guardar. Verifique duplicados e tente novamente.");
                }
            }
            return View(material);
        }

        // GET: Materiais/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var material = await _context.Materiais
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (material == null) return NotFound();

            return View(material);
        }

        // POST: Materiais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var material = await _context.Materiais.FindAsync(id);
                if (material != null) _context.Materiais.Remove(material);

                await _context.SaveChangesAsync();
                TempData["Success"] = "Material removido.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                // FK Restrict: não permite apagar materiais com movimentos
                TempData["Error"] = "Não é possível apagar este material: existem movimentos registados.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        private bool MaterialExists(int id)
        {
            return _context.Materiais.Any(e => e.Id == id);
        }
    }
}