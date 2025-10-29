namespace GestaoObras.Domain.Entities;

public class Pagamento
{
    public int Id { get; set; }
    public int ObraId { get; set; }
    public string Nome { get; set; } = null!;
    public decimal Valor { get; set; }
    public DateTime DataHora { get; set; }

    public Obra? Obra { get; set; }
}