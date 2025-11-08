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
        public MaosDeObraController(ObrasDbContext context) => _context = context;

        [HttpGet] public IActionResult Index() => NotFound();
        [HttpGet] public IActionResult Details(int id) => NotFound();
        [HttpGet] public IActionResult Create() => NotFound();
        [HttpGet] public IActionResult Edit(int id) => NotFound();
        [HttpGet] public IActionResult Delete(int id) => NotFound();

        private static DateTime ToUtc(DateTime dt)
        {
            if (dt == default) return DateTime.UtcNow;
            if (dt.Kind == DateTimeKind.Unspecified) dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
            return dt.ToUniversalTime();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ObraId,Nome,HorasTrabalhadas,DataHora")] MaoDeObra maoDeObra, string? returnUrl)
        {
            maoDeObra.DataHora = ToUtc(maoDeObra.DataHora);

            if (!ModelState.IsValid) return RedirectToObra(maoDeObra.ObraId, returnUrl, "mao");

            _context.MaosDeObra.Add(maoDeObra);
            await _context.SaveChangesAsync();
            return RedirectToObra(maoDeObra.ObraId, returnUrl, "mao");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            var mao = await _context.MaosDeObra.FindAsync(id);
            if (mao == null) return NotFound();

            var obraId = mao.ObraId;
            _context.MaosDeObra.Remove(mao);
            await _context.SaveChangesAsync();
            return RedirectToObra(obraId, returnUrl, "mao");
        }

        private IActionResult RedirectToObra(int obraId, string? returnUrl, string tab)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);
            var url = Url.Action("Details", "Obras", new { id = obraId, tab });
            return Redirect(url ?? "/Obras/Index");
        }
    }
}