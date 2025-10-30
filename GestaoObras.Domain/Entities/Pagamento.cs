using System.ComponentModel.DataAnnotations;

namespace GestaoObras.Domain.Entities;

public class Pagamento
{
    public int Id { get; set; }

    [Display(Name = "Obra")]
    [Required(ErrorMessage = "A {0} é obrigatória.")]
    public int ObraId { get; set; }

    [Display(Name = "Nome")]
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [StringLength(160, ErrorMessage = "O {0} não pode exceder {1} caracteres.")]
    public string Nome { get; set; } = null!;

    [Display(Name = "Valor (€)")]
    [Range(0.01, 9999999, ErrorMessage = "O {0} deve ser positivo.")]
    [DataType(DataType.Currency)]
    public decimal Valor { get; set; }

    [Display(Name = "Data e Hora")]
    [DataType(DataType.Date)]
    public DateTime DataHora { get; set; }

    public Obra? Obra { get; set; }
}