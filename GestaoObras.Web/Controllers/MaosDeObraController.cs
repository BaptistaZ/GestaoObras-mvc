using GestaoObras.Data.Context;
using GestaoObras.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoObras.Web.Controllers;

public class MaosDeObraController : Controller
{
    private readonly ObrasDbContext _context;
    private const string Tab = "mao";

    public MaosDeObraController(ObrasDbContext context) => _context = context;

    /// <summary>
    /// Converte datas para UTC de forma robusta:
    /// - se vier default => agora (UTC)
    /// - se vier Unspecified => assume Local e converte
    /// </summary>
    private static DateTime NormalizeToUtc(DateTime dt)
    {
        if (dt == default) return DateTime.UtcNow;
        if (dt.Kind == DateTimeKind.Unspecified)
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
        return dt.ToUniversalTime();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("ObraId,Nome,HorasTrabalhadas,DataHora")] MaoDeObra maoDeObra,
        string? returnUrl,
        CancellationToken ct)
    {
        // Normalizações de input
        maoDeObra.Nome = (maoDeObra.Nome ?? string.Empty).Trim();
        maoDeObra.HorasTrabalhadas = Math.Round(maoDeObra.HorasTrabalhadas, 2);
        maoDeObra.DataHora = NormalizeToUtc(maoDeObra.DataHora);

        // Validações adicionais de servidor
        if (!await _context.Obras.AsNoTracking().AnyAsync(o => o.Id == maoDeObra.ObraId, ct))
        {
            TempData["FormError_mao"] = "A obra indicada não existe.";
            return RedirectToObra(maoDeObra.ObraId, returnUrl);
        }
        if (maoDeObra.HorasTrabalhadas <= 0)
        {
            ModelState.AddModelError(nameof(MaoDeObra.HorasTrabalhadas),
                "As horas trabalhadas devem ser superiores a 0.");
        }

        if (!ModelState.IsValid)
        {
            TempData["FormError_mao"] = string.Join(" | ",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToObra(maoDeObra.ObraId, returnUrl);
        }

        try
        {
            _context.MaosDeObra.Add(maoDeObra);
            await _context.SaveChangesAsync(ct);
            TempData["Success"] = "Registo de mão-de-obra adicionado.";
        }
        catch (DbUpdateException)
        {
            TempData["FormError_mao"] = "Não foi possível guardar o registo. Tente novamente.";
        }

        return RedirectToObra(maoDeObra.ObraId, returnUrl);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl, CancellationToken ct)
    {
        var mao = await _context.MaosDeObra.FindAsync(new object?[] { id }, ct);
        if (mao == null) return NotFound();

        var obraId = mao.ObraId;

        try
        {
            _context.MaosDeObra.Remove(mao);
            await _context.SaveChangesAsync(ct);
            TempData["Success"] = "Registo de mão-de-obra removido.";
        }
        catch (DbUpdateException)
        {
            TempData["FormError_mao"] = "Não foi possível remover o registo. Tente novamente.";
        }

        return RedirectToObra(obraId, returnUrl);
    }

    private IActionResult RedirectToObra(int obraId, string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        var url = Url.Action("Details", "Obras", new { id = obraId, tab = Tab });
        return Redirect(url ?? "/Obras/Index");
    }
}