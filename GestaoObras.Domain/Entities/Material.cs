using System.ComponentModel.DataAnnotations;

namespace GestaoObras.Domain.Entities;

public class Material
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descricao { get; set; }

    [Display(Name = "Stock Disponível")]
    [Range(0, int.MaxValue)]
    public int StockDisponivel { get; set; }

    // Navegação
    public ICollection<MovimentoMaterial> Movimentos { get; set; } = new List<MovimentoMaterial>();
}