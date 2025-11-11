using System.ComponentModel.DataAnnotations;

namespace GestaoObras.Domain.Entities;

public class Pagamento
{
    public int Id { get; set; }

    [Required]
    public int ObraId { get; set; }

    [Required]
    [StringLength(160)]
    public string Nome { get; set; } = null!;

    [Display(Name = "Valor (€)")]
    [Range(0.01, 9999999)]
    [DataType(DataType.Currency)]
    public decimal Valor { get; set; }

    [Display(Name = "Data e Hora")]
    [DataType(DataType.Date)]
    public DateTime DataHora { get; set; }

    public Obra? Obra { get; set; }
}