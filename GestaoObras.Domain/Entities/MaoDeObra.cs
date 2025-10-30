using System.ComponentModel.DataAnnotations;

namespace GestaoObras.Domain.Entities;

public class MaoDeObra
{
    public int Id { get; set; }

    [Display(Name = "Obra")]
    [Required(ErrorMessage = "A {0} é obrigatória.")]
    public int ObraId { get; set; }

    [Display(Name = "Nome do Trabalhador")]
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [StringLength(120, ErrorMessage = "O {0} não pode exceder {1} caracteres.")]
    public string Nome { get; set; } = null!;

    [Display(Name = "Horas Trabalhadas")]
    [Range(0.25, 24, ErrorMessage = "As {0} devem estar entre {1} e {2}.")]
    public decimal HorasTrabalhadas { get; set; }

    [Display(Name = "Data e Hora")]
    [DataType(DataType.DateTime)]
    public DateTime DataHora { get; set; }

    public Obra? Obra { get; set; }
}