namespace GestaoObras.Web.Models
{
    public class DashboardVM
    {
        public int ObrasAtivas { get; set; }
        public int ClientesTotal { get; set; }
        public int MateriaisTotal { get; set; }
        public int MovimentosHoje { get; set; }

        public IEnumerable<UltimoMovimentoItem> UltimosMovimentos { get; set; } = Array.Empty<UltimoMovimentoItem>();
    }

    public class UltimoMovimentoItem
    {
        public DateTime Data { get; set; }
        public string Obra { get; set; } = "";
        public string Material { get; set; } = "";
        public string Operacao { get; set; } = ""; // "Entrada" | "Saida"
        public int Quantidade { get; set; }
    }
}