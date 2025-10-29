namespace GestaoObras.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string NIF { get; set; } = null!;
    public string Morada { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Telefone { get; set; } = null!;

    public ICollection<Obra> Obras { get; set; } = new List<Obra>();
}