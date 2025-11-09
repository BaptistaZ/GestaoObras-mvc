using System;
using System.Linq;
using System.Threading.Tasks;
using GestaoObras.Data.Context;
using GestaoObras.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoObras.Web.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ObrasDbContext _context;

        public ClientesController(ObrasDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index(
            string? search,
            string sort = "nome_asc",
            int page = 1,
            int pageSize = 10)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            // base query
            var q = _context.Clientes.AsNoTracking();

            // filtro (pesquisa por nome, email, telemóvel)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(c =>
                    EF.Functions.ILike(c.Nome, $"%{s}%") ||
                    EF.Functions.ILike(c.Email ?? "", $"%{s}%") ||
                    EF.Functions.ILike(c.Telefone ?? "", $"%{s}%"));
            }

            // ordenação
            q = sort switch
            {
                "nome_desc" => q.OrderByDescending(c => c.Nome),
                "email_asc" => q.OrderBy(c => c.Email),
                "email_desc" => q.OrderByDescending(c => c.Email),
                _ => q.OrderBy(c => c.Nome) // nome_asc (default)
            };

            // paginação
            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = total;

            return View(items);
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,NIF,Morada,Email,Telefone")] Cliente cliente)
        {
            // valida duplicado por NIF
            if (await _context.Clientes.AnyAsync(c => c.NIF == cliente.NIF))
                ModelState.AddModelError(nameof(cliente.NIF), "Já existe um cliente com este NIF.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cliente criado com sucesso.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "Não foi possível guardar. Verifique duplicados e tente novamente.");
                }
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,NIF,Morada,Email,Telefone")] Cliente cliente)
        {
            if (id != cliente.Id) return NotFound();

            // valida duplicado por NIF (exclui o próprio)
            if (await _context.Clientes.AnyAsync(c => c.Id != id && c.NIF == cliente.NIF))
                ModelState.AddModelError(nameof(cliente.NIF), "Já existe um cliente com este NIF.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cliente atualizado.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id)) return NotFound(); else throw;
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "Não foi possível guardar. Verifique duplicados e tente novamente.");
                }
            }
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cliente removido.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}