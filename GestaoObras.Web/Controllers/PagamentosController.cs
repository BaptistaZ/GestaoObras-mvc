using GestaoObras.Data.Context;
using GestaoObras.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoObras.Web.Controllers;

public class PagamentosController : Controller
{
    private readonly ObrasDbContext _context;
    public PagamentosController(ObrasDbContext context) => _context = context;

    private static DateTime ToUtc(DateTime dt)
    {
        if (dt == default) return DateTime.UtcNow;
        if (dt.Kind == DateTimeKind.Unspecified)
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
        return dt.ToUniversalTime();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("ObraId,Nome,Valor,DataHora")] Pagamento pagamento,
        string? returnUrl,
        CancellationToken ct)
    {
        pagamento.DataHora = ToUtc(pagamento.DataHora);

        if (!ModelState.IsValid)
        {
            // devolve mensagem compacta para a aba "pag"
            TempData["FormError_pag"] = string.Join(" | ",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToObra(pagamento.ObraId, returnUrl, "pag");
        }

        try
        {
            _context.Pagamentos.Add(pagamento);
            await _context.SaveChangesAsync(ct);
            TempData["Success"] = "Pagamento registado.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Não foi possível registar o pagamento. Tente novamente.";
        }

        return RedirectToObra(pagamento.ObraId, returnUrl, "pag");
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        int id,
        string? returnUrl,
        CancellationToken ct)
    {
        var pagamento = await _context.Pagamentos.FindAsync([id], ct);
        if (pagamento == null) return NotFound();

        var obraId = pagamento.ObraId;

        try
        {
            _context.Pagamentos.Remove(pagamento);
            await _context.SaveChangesAsync(ct);
            TempData["Success"] = "Pagamento removido.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Não foi possível remover o pagamento. Tente novamente.";
        }

        return RedirectToObra(obraId, returnUrl, "pag");
    }

    // Redireciona de forma segura para a obra (aba específica)
    private IActionResult RedirectToObra(int obraId, string? returnUrl, string tab)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        var url = Url.Action("Details", "Obras", new { id = obraId, tab });
        return Redirect(url ?? "/Obras/Index");
    }
}