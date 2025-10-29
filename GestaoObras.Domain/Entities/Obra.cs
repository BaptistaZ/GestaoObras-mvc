namespace GestaoObras.Domain.Entities;

public class Obra
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public int ClienteId { get; set; }
    public string Morada { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool Ativa { get; set; }

    // Navegação (sem EF por agora)
    public Cliente? Cliente { get; set; }
    public ICollection<MovimentoMaterial> MovimentosMaterial { get; set; } = new List<MovimentoMaterial>();
    public ICollection<MaoDeObra> MaosDeObra { get; set; } = new List<MaoDeObra>();
    public ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
}