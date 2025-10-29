namespace GestaoObras.Domain.Entities;

public class Material
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public int StockDisponivel { get; set; }

    public ICollection<MovimentoMaterial> MovimentosMaterial { get; set; } = new List<MovimentoMaterial>();
}