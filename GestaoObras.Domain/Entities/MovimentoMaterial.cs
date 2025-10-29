using GestaoObras.Domain.Enums;

namespace GestaoObras.Domain.Entities;

public class MovimentoMaterial
{
    public int Id { get; set; }

    public int ObraId { get; set; }
    public int MaterialId { get; set; }
    public int Quantidade { get; set; }
    public DateTime DataHora { get; set; }
    public OperacaoStock Operacao { get; set; }

    // Navegação (opcional agora)
    public Obra? Obra { get; set; }
    public Material? Material { get; set; }
}