using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestaoObras.Data.Context;
using GestaoObras.Domain.Entities;

namespace GestaoObras.Web.Controllers
{
    public class MovimentosMaterialController : Controller
    {
        private readonly ObrasDbContext _context;

        public MovimentosMaterialController(ObrasDbContext context)
        {
            _context = context;
        }

        // GET: MovimentosMaterial
        public async Task<IActionResult> Index()
        {
            var obrasDbContext = _context.MovimentosMaterial.Include(m => m.Material).Include(m => m.Obra);
            return View(await obrasDbContext.ToListAsync());
        }

        // GET: MovimentosMaterial/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimentoMaterial = await _context.MovimentosMaterial
                .Include(m => m.Material)
                .Include(m => m.Obra)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movimentoMaterial == null)
            {
                return NotFound();
            }

            return View(movimentoMaterial);
        }

        // GET: MovimentosMaterial/Create
        public IActionResult Create()
        {
            ViewData["MaterialId"] = new SelectList(_context.Materiais, "Id", "Nome");
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Descricao");
            return View();
        }

        // POST: MovimentosMaterial/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ObraId,MaterialId,Quantidade,DataHora,Operacao")] MovimentoMaterial movimentoMaterial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movimentoMaterial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaterialId"] = new SelectList(_context.Materiais, "Id", "Nome", movimentoMaterial.MaterialId);
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Descricao", movimentoMaterial.ObraId);
            return View(movimentoMaterial);
        }

        // GET: MovimentosMaterial/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimentoMaterial = await _context.MovimentosMaterial.FindAsync(id);
            if (movimentoMaterial == null)
            {
                return NotFound();
            }
            ViewData["MaterialId"] = new SelectList(_context.Materiais, "Id", "Nome", movimentoMaterial.MaterialId);
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Descricao", movimentoMaterial.ObraId);
            return View(movimentoMaterial);
        }

        // POST: MovimentosMaterial/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ObraId,MaterialId,Quantidade,DataHora,Operacao")] MovimentoMaterial movimentoMaterial)
        {
            if (id != movimentoMaterial.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movimentoMaterial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovimentoMaterialExists(movimentoMaterial.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaterialId"] = new SelectList(_context.Materiais, "Id", "Nome", movimentoMaterial.MaterialId);
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Descricao", movimentoMaterial.ObraId);
            return View(movimentoMaterial);
        }

        // GET: MovimentosMaterial/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimentoMaterial = await _context.MovimentosMaterial
                .Include(m => m.Material)
                .Include(m => m.Obra)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movimentoMaterial == null)
            {
                return NotFound();
            }

            return View(movimentoMaterial);
        }

        // POST: MovimentosMaterial/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movimentoMaterial = await _context.MovimentosMaterial.FindAsync(id);
            if (movimentoMaterial != null)
            {
                _context.MovimentosMaterial.Remove(movimentoMaterial);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovimentoMaterialExists(int id)
        {
            return _context.MovimentosMaterial.Any(e => e.Id == id);
        }
    }
}
