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
    public class MaosDeObraController : Controller
    {
        private readonly ObrasDbContext _context;

        public MaosDeObraController(ObrasDbContext context)
        {
            _context = context;
        }

        // GET: MaosDeObra
        public async Task<IActionResult> Index()
        {
            var obrasDbContext = _context.MaosDeObra.Include(m => m.Obra);
            return View(await obrasDbContext.ToListAsync());
        }

        // GET: MaosDeObra/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maoDeObra = await _context.MaosDeObra
                .Include(m => m.Obra)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maoDeObra == null)
            {
                return NotFound();
            }

            return View(maoDeObra);
        }

        // GET: MaosDeObra/Create
        public IActionResult Create()
        {
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Descricao");
            return View();
        }

        // POST: MaosDeObra/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ObraId,Nome,HorasTrabalhadas,DataHora")] MaoDeObra maoDeObra)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maoDeObra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Descricao", maoDeObra.ObraId);
            return View(maoDeObra);
        }

        // GET: MaosDeObra/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maoDeObra = await _context.MaosDeObra.FindAsync(id);
            if (maoDeObra == null)
            {
                return NotFound();
            }
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Descricao", maoDeObra.ObraId);
            return View(maoDeObra);
        }

        // POST: MaosDeObra/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ObraId,Nome,HorasTrabalhadas,DataHora")] MaoDeObra maoDeObra)
        {
            if (id != maoDeObra.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maoDeObra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaoDeObraExists(maoDeObra.Id))
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
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Descricao", maoDeObra.ObraId);
            return View(maoDeObra);
        }

        // GET: MaosDeObra/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maoDeObra = await _context.MaosDeObra
                .Include(m => m.Obra)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maoDeObra == null)
            {
                return NotFound();
            }

            return View(maoDeObra);
        }

        // POST: MaosDeObra/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maoDeObra = await _context.MaosDeObra.FindAsync(id);
            if (maoDeObra != null)
            {
                _context.MaosDeObra.Remove(maoDeObra);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaoDeObraExists(int id)
        {
            return _context.MaosDeObra.Any(e => e.Id == id);
        }
    }
}
