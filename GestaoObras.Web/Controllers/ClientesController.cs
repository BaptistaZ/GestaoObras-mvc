using System.Linq.Expressions;
using GestaoObras.Data.Context;
using GestaoObras.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoObras.Web.Controllers;

public class ClientesController : Controller
{
    private readonly ObrasDbContext _context;

    // Paginação
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    public ClientesController(ObrasDbContext context) => _context = context;

    // GET: Clientes
    public async Task<IActionResult> Index(
        string? search,
        string sort = "nome_asc",
        int page = 1,
        int pageSize = DefaultPageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        IQueryable<Cliente> q = _context.Clientes.AsNoTracking();

        // Filtro (Nome, Email, Telefone, NIF)
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            // Nota: ILike é específico de PostgreSQL (case-insensitive)
            q = q.Where(c =>
                EF.Functions.ILike(c.Nome, $"%{s}%") ||
                EF.Functions.ILike(c.Email ?? "", $"%{s}%") ||
                EF.Functions.ILike(c.Telefone ?? "", $"%{s}%") ||
                EF.Functions.ILike(c.NIF ?? "", $"%{s}%"));
        }

        // Ordenação determinística (ThenBy Id)
        q = ApplySorting(q, sort).ThenBy(c => c.Id);

        var total = await q.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling((double)total / pageSize));
        if (page > totalPages) page = totalPages;

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
        if (id is null) return NotFound();

        var cliente = await _context.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (cliente is null) return NotFound();

        return View(cliente);
    }

    // GET: Clientes/Create
    public IActionResult Create() => View();

    // POST: Clientes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nome,NIF,Morada,Email,Telefone")] Cliente cliente)
    {
        Normalize(cliente);

        // NIF único
        if (await _context.Clientes.AnyAsync(c => c.NIF == cliente.NIF))
            ModelState.AddModelError(nameof(cliente.NIF), "Já existe um cliente com este NIF.");

        if (!ModelState.IsValid) return View(cliente);

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
            return View(cliente);
        }
    }

    // GET: Clientes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null) return NotFound();

        return View(cliente);
    }

    // POST: Clientes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,NIF,Morada,Email,Telefone")] Cliente cliente)
    {
        if (id != cliente.Id) return NotFound();

        Normalize(cliente);

        // NIF único, excluindo o próprio
        if (await _context.Clientes.AnyAsync(c => c.Id != id && c.NIF == cliente.NIF))
            ModelState.AddModelError(nameof(cliente.NIF), "Já existe um cliente com este NIF.");

        if (!ModelState.IsValid) return View(cliente);

        try
        {
            _context.Update(cliente);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cliente atualizado.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClienteExists(cliente.Id)) return NotFound();
            throw;
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível guardar. Verifique duplicados e tente novamente.");
            return View(cliente);
        }
    }

    // GET: Clientes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var cliente = await _context.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (cliente is null) return NotFound();

        return View(cliente);
    }

    // POST: Clientes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Bloquear se houver obras associadas
        var temObras = await _context.Obras.AsNoTracking().AnyAsync(o => o.ClienteId == id);
        if (temObras)
        {
            TempData["Error"] = "Não é possível apagar este cliente: existem obras associadas.";
            return RedirectToAction(nameof(Index));
        }

        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null) return RedirectToAction(nameof(Index));

        try
        {
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cliente removido.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Não foi possível apagar o cliente (restrições de integridade).";
        }

        return RedirectToAction(nameof(Index));
    }

    // ---------- Helpers ----------

    private static void Normalize(Cliente c)
    {
        // Trim + normalizações simples (evitam duplicados com espaços, etc.)
        c.Nome = c.Nome?.Trim() ?? "";
        c.Morada = c.Morada?.Trim() ?? "";
        c.Email = c.Email?.Trim() ?? "";
        c.Telefone = c.Telefone?.Trim() ?? "";
        c.NIF = c.NIF?.Trim() ?? "";
    }

    private static IOrderedQueryable<Cliente> ApplySorting(IQueryable<Cliente> q, string sort)
    {
        // Aceita padrões: nome_asc, nome_desc, nif_asc, nif_desc, email_asc, email_desc
        var (field, dir) = ParseSort(sort);

        Expression<Func<Cliente, object>> keySelector = field switch
        {
            "nif" => c => c.NIF!,
            "email" => c => c.Email!,
            _ => c => c.Nome! // default: nome
        };

        return dir == "desc" ? q.OrderByDescending(keySelector)
                             : q.OrderBy(keySelector);
    }

    private static (string field, string dir) ParseSort(string sort)
    {
        var parts = (sort ?? "").Split('_', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var field = parts.Length > 0 ? parts[0].ToLowerInvariant() : "nome";
        var dir = parts.Length > 1 ? parts[1].ToLowerInvariant() : "asc";
        if (dir != "asc" && dir != "desc") dir = "asc";
        return (field, dir);
    }

    private bool ClienteExists(int id) => _context.Clientes.Any(e => e.Id == id);
}