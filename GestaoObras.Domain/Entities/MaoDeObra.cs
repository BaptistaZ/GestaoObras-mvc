namespace GestaoObras.Domain.Entities;

public class MaoDeObra
{
    public int Id { get; set; }
    public int ObraId { get; set; }
    public string Nome { get; set; } = null!;
    public decimal HorasTrabalhadas { get; set; }   // ex.: 7.50
    public DateTime DataHora { get; set; }

    public Obra? Obra { get; set; }
}