using System.ComponentModel.DataAnnotations;
using GestaoObras.Domain.Enums;

namespace GestaoObras.Domain.Entities;

public class MovimentoMaterial
{
    public int Id { get; set; }

    [Display(Name = "Obra")]
    [Required(ErrorMessage = "A {0} é obrigatória.")]
    public int ObraId { get; set; }

    [Display(Name = "Material")]
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    public int MaterialId { get; set; }

    [Display(Name = "Quantidade")]
    [Range(1, int.MaxValue, ErrorMessage = "A {0} deve ser pelo menos {1}.")]
    public int Quantidade { get; set; }

    [Display(Name = "Data e Hora")]
    [DataType(DataType.DateTime)]
    public DateTime DataHora { get; set; }

    [Display(Name = "Operação")]
    [Required(ErrorMessage = "A {0} é obrigatória.")]
    public OperacaoStock Operacao { get; set; }

    public Obra? Obra { get; set; }
    public Material? Material { get; set; }
}