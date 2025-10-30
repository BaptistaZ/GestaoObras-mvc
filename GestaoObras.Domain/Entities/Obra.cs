using System.ComponentModel.DataAnnotations;

namespace GestaoObras.Domain.Entities;

public class Obra
{
    public int Id { get; set; }

    [Display(Name = "Nome")]
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [StringLength(160, ErrorMessage = "O {0} não pode exceder {1} caracteres.")]
    public string Nome { get; set; } = null!;

    [Display(Name = "Descrição")]
    [Required(ErrorMessage = "A {0} é obrigatória.")]
    [StringLength(500, ErrorMessage = "A {0} não pode exceder {1} caracteres.")]
    public string Descricao { get; set; } = null!;

    [Display(Name = "Cliente")]
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    public int ClienteId { get; set; }

    [Display(Name = "Morada da Obra")]
    [Required(ErrorMessage = "A {0} é obrigatória.")]
    [StringLength(200, ErrorMessage = "A {0} não pode exceder {1} caracteres.")]
    public string Morada { get; set; } = null!;

    [Display(Name = "Latitude")]
    [Range(-90, 90, ErrorMessage = "A {0} deve estar entre {1} e {2}.")]
    public double Latitude { get; set; }

    [Display(Name = "Longitude")]
    [Range(-180, 180, ErrorMessage = "A {0} deve estar entre {1} e {2}.")]
    public double Longitude { get; set; }

    [Display(Name = "Ativa")]
    public bool Ativa { get; set; }

    public Cliente? Cliente { get; set; }
    public ICollection<MovimentoMaterial> MovimentosMaterial { get; set; } = new List<MovimentoMaterial>();
    public ICollection<MaoDeObra> MaosDeObra { get; set; } = new List<MaoDeObra>();
    public ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
}