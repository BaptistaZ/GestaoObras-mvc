using System.ComponentModel.DataAnnotations;

namespace GestaoObras.Domain.Entities;

public class MaoDeObra
{
    public int Id { get; set; }

    [Required]
    public int ObraId { get; set; }

    [Display(Name = "Nome do Trabalhador")]
    [Required]
    [StringLength(120)]
    public string Nome { get; set; } = null!;

    [Display(Name = "Horas Trabalhadas")]
    [Range(0.25, 24)]
    public decimal HorasTrabalhadas { get; set; }

    [Display(Name = "Data e Hora")]
    [DataType(DataType.DateTime)]
    public DateTime DataHora { get; set; }

    // Navegação
    public Obra? Obra { get; set; }
}