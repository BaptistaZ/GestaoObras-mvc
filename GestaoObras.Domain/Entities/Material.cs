using System.ComponentModel.DataAnnotations;

namespace GestaoObras.Domain.Entities;

public class Material
{
    public int Id { get; set; }

    [Display(Name = "Nome")]
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [StringLength(120, ErrorMessage = "O {0} não pode exceder {1} caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "Descrição")]
    [StringLength(500, ErrorMessage = "A {0} não pode exceder {1} caracteres.")]
    public string? Descricao { get; set; }

    [Display(Name = "Stock Disponível")]
    [Range(0, int.MaxValue, ErrorMessage = "O {0} deve ser um número igual ou superior a {1}.")]
    public int StockDisponivel { get; set; }

    public ICollection<MovimentoMaterial> Movimentos { get; set; } = new List<MovimentoMaterial>();
}