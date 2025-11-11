using System.ComponentModel.DataAnnotations;
using GestaoObras.Domain.Enums;

namespace GestaoObras.Domain.Entities;

public class MovimentoMaterial
{
    public int Id { get; set; }

    [Required]
    public int ObraId { get; set; }

    [Required]
    public int MaterialId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantidade { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime DataHora { get; set; }

    [Required]
    public OperacaoStock Operacao { get; set; }

    // Navegação
    public Obra? Obra { get; set; }
    public Material? Material { get; set; }
}