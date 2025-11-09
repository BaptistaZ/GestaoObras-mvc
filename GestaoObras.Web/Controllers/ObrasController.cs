using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestaoObras.Data.Context;
using GestaoObras.Domain.Entities;

namespace GestaoObras.Web.Controllers
{
    public class ObrasController : Controller
    {
        private readonly ObrasDbContext _context;

        public ObrasController(ObrasDbContext context)
        {
            _context = context;
        }

        // GET: Obras
        public async Task<IActionResult> Index()
        {
            var query = _context.Obras
                .AsNoTracking()
                .Include(o => o.Cliente)
                .OrderBy(o => o.Nome);
            return View(await query.ToListAsync());
        }

        // GET: Obras/Details/5
        public async Task<IActionResult> Details(int id, string? tab = null)
        {
            var obra = await _context.Obras
                .AsNoTracking()
                .Include(o => o.Cliente)
                .Include(o => o.MovimentosMaterial).ThenInclude(m => m.Material)
                .Include(o => o.MaosDeObra)
                .Include(o => o.Pagamentos)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (obra == null) return NotFound();

            ViewBag.ActiveTab = tab;

            // Para o dropdown de materiais (form "Novo movimento")
            ViewBag.MaterialId = new SelectList(
                await _context.Materiais
                    .AsNoTracking()
                    .OrderBy(m => m.Nome)
                    .ToListAsync(),
                "Id", "Nome"
            );

            // URL atual (inclui ?tab=...) para voltar após guardar no Edit
            ViewBag.CurrentUrl = HttpContext?.Request?.Path + HttpContext?.Request?.QueryString;

            return View(obra);
        }

        // GET: Obras/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(
                _context.Clientes.AsNoTracking().OrderBy(c => c.Nome),
                "Id", "Nome"
            );
            return View();
        }

        // POST: Obras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,ClienteId,Morada,Latitude,Longitude,Ativa")] Obra obra)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ClienteId"] = new SelectList(
                    _context.Clientes.AsNoTracking().OrderBy(c => c.Nome),
                    "Id", "Nome", obra.ClienteId
                );
                return View(obra);
            }

            _context.Add(obra);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Obras/Edit/5
        public async Task<IActionResult> Edit(int? id, string? returnUrl = null)
        {
            if (id == null) return NotFound();

            var obra = await _context.Obras.FindAsync(id);
            if (obra == null) return NotFound();

            ViewData["ClienteId"] = new SelectList(
                _context.Clientes.AsNoTracking().OrderBy(c => c.Nome),
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
            string? returnUrl)
        {
            if (id != obra.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["ClienteId"] = new SelectList(
                    _context.Clientes.AsNoTracking().OrderBy(c => c.Nome),
                    "Id", "Nome", obra.ClienteId
                );
                ViewBag.ReturnUrl = returnUrl;
                return View(obra);
            }

            try
            {
                _context.Update(obra);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Obras.Any(e => e.Id == obra.Id)) return NotFound();
                throw;
            }

            // Prioridade ao returnUrl (mantém a mesma aba dos detalhes)
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            // Fallback
            return RedirectToAction("Details", new { id = obra.Id });
        }

        // GET: Obras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var obra = await _context.Obras
                .AsNoTracking()
                .Include(o => o.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (obra == null) return NotFound();

            return View(obra);
        }

        // POST: Obras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var obra = await _context.Obras.FindAsync(id);
            if (obra != null)
            {
                _context.Obras.Remove(obra);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ObraExists(int id) => _context.Obras.Any(e => e.Id == id);
    }
}