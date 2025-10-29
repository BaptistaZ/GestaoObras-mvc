using System.ComponentModel.DataAnnotations;

namespace GestaoObras.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Nome { get; set; } = null!;

    [Required, StringLength(9, MinimumLength = 9, ErrorMessage = "O NIF deve ter 9 dígitos")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "O NIF deve ter 9 dígitos")]
    public string NIF { get; set; } = null!;

    [Required, StringLength(200)]
    public string Morada { get; set; } = null!;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = null!;

    [Required, Phone, StringLength(20)]
    public string Telefone { get; set; } = null!;

    public ICollection<Obra> Obras { get; set; } = new List<Obra>();
}