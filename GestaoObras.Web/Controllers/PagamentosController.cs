using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoObras.Data.Context;
using GestaoObras.Domain.Entities;

namespace GestaoObras.Web.Controllers
{
    public class PagamentosController : Controller
    {
        private readonly ObrasDbContext _context;

        public PagamentosController(ObrasDbContext context)
        {
            _context = context;
        }

        // Desativar páginas não usadas
        [HttpGet] public IActionResult Index() => NotFound();
        [HttpGet] public IActionResult Details(int id) => NotFound();
        [HttpGet] public IActionResult Create() => NotFound();
        [HttpGet] public IActionResult Edit(int id) => NotFound();
        [HttpGet] public IActionResult Delete(int id) => NotFound();

        // POST: Pagamentos/Create (usado dentro de Obras/Details)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ObraId,Nome,Valor,DataHora")] Pagamento pagamento,
            string? returnUrl)
        {
            if (pagamento.DataHora == default)
                pagamento.DataHora = DateTime.Now;

            if (!ModelState.IsValid)
                return RedirectToObra(pagamento.ObraId, returnUrl, "pag");

            _context.Pagamentos.Add(pagamento);
            await _context.SaveChangesAsync();

            return RedirectToObra(pagamento.ObraId, returnUrl, "pag");
        }

        // POST: Pagamentos/DeleteConfirmed (usado dentro de Obras/Details)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            var pagamento = await _context.Pagamentos.FindAsync(id);
            if (pagamento != null)
            {
                var obraId = pagamento.ObraId;
                _context.Pagamentos.Remove(pagamento);
                await _context.SaveChangesAsync();

                return RedirectToObra(obraId, returnUrl, "pag");
            }

            return NotFound();
        }

        // Helper comum: redireciona sempre para a obra/tab correta
        private IActionResult RedirectToObra(int obraId, string? returnUrl, string tab)
        {
            // ⚠️ Garante que o returnUrl é mesmo local (evita loop se for nulo ou inválido)
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            // ✅ fallback automático: volta sempre para os detalhes da obra e aba certa
            var url = Url.Action("Details", "Obras", new { id = obraId, tab });
            return Redirect(url ?? "/Obras/Index");
        }
    }
}