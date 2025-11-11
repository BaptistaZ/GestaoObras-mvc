using System;

namespace GestaoObras.Web.Models
{
    /// ViewModel usado no Dashboard: KPIs + últimos movimentos.
    public sealed class DashboardVM
    {
        public int ObrasAtivas { get; set; }
        public int ClientesTotal { get; set; }
        public int MateriaisTotal { get; set; }
        public int MovimentosHoje { get; set; }

        /// Últimos movimentos registados, ordenados por data desc.
        public IEnumerable<UltimoMovimentoItem> UltimosMovimentos { get; set; }
            = Array.Empty<UltimoMovimentoItem>();
    }

    /// Item individual da lista Últimos Movimentos no Dashboard.
    public sealed class UltimoMovimentoItem
    {
        public DateTime Data { get; set; }
        public string Obra { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;

        /// "Entrada" ou "Saida".
        public string Operacao { get; set; } = string.Empty;

        public int Quantidade { get; set; }
    }
}