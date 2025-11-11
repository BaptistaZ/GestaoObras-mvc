using System.ComponentModel.DataAnnotations;

namespace GestaoObras.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }

    [Display(Name = "Nome")]
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [StringLength(120)]
    public string Nome { get; set; } = null!;

    [Display(Name = "NIF")]
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "O NIF deve ter 9 dígitos.")]
    public string NIF { get; set; } = null!;

    [Display(Name = "Morada")]
    [Required(ErrorMessage = "A {0} é obrigatória.")]
    [StringLength(200)]
    public string Morada { get; set; } = null!;

    [Display(Name = "Email")]
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [EmailAddress(ErrorMessage = "Introduza um {0} válido.")]
    [StringLength(150)]
    public string Email { get; set; } = null!;

    [Display(Name = "Telefone")]
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [Phone(ErrorMessage = "Introduza um {0} válido.")]
    [StringLength(20)]
    public string Telefone { get; set; } = null!;

    // Navegação
    public ICollection<Obra> Obras { get; set; } = new List<Obra>();
}