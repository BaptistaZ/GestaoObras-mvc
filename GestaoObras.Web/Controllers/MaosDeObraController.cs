using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        // Desativar páginas não usadas
        [HttpGet] public IActionResult Index() => NotFound();
        [HttpGet] public IActionResult Details(int id) => NotFound();
        [HttpGet] public IActionResult Create() => NotFound();
        [HttpGet] public IActionResult Edit(int id) => NotFound();
        [HttpGet] public IActionResult Delete(int id) => NotFound();

        // POST: MaosDeObra/Create (usado dentro de Obras/Details)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ObraId,Nome,HorasTrabalhadas,DataHora")] MaoDeObra maoDeObra,
            string? returnUrl)
        {
            if (maoDeObra.DataHora == default)
                maoDeObra.DataHora = DateTime.Now;

            if (!ModelState.IsValid)
                return RedirectToObra(maoDeObra.ObraId, returnUrl, "mao");

            _context.MaosDeObra.Add(maoDeObra);
            await _context.SaveChangesAsync();

            return RedirectToObra(maoDeObra.ObraId, returnUrl, "mao");
        }

        // POST: MaosDeObra/DeleteConfirmed (usado dentro de Obras/Details)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            var mao = await _context.MaosDeObra.FindAsync(id);
            if (mao != null)
            {
                var obraId = mao.ObraId;
                _context.MaosDeObra.Remove(mao);
                await _context.SaveChangesAsync();

                return RedirectToObra(obraId, returnUrl, "mao");
            }

            return NotFound();
        }

        // Helper comum: redireciona sempre para a obra/tab correta
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