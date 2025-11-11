using System.ComponentModel.DataAnnotations;

namespace GestaoObras.Domain.Entities;

public class Obra
{
    public int Id { get; set; }

    [Required]
    [StringLength(160)]
    public string Nome { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string Descricao { get; set; } = null!;

    [Required]
    public int ClienteId { get; set; }

    [Display(Name = "Morada da Obra")]
    [Required]
    [StringLength(200)]
    public string Morada { get; set; } = null!;

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }

    public bool Ativa { get; set; }

    // Navegações
    public Cliente? Cliente { get; set; }
    public ICollection<MovimentoMaterial> MovimentosMaterial { get; set; } = new List<MovimentoMaterial>();
    public ICollection<MaoDeObra> MaosDeObra { get; set; } = new List<MaoDeObra>();
    public ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
}