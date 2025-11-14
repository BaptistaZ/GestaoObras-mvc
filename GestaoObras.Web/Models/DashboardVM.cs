using System;
using System.Collections.Generic;

namespace GestaoObras.Web.Models
{
    /// ViewModel usado no Dashboard: KPIs + últimos movimentos + resumo de obras.
    public sealed class DashboardVM
    {
        public int ObrasAtivas { get; set; }
        public int ClientesTotal { get; set; }
        public int MateriaisTotal { get; set; }
        public int MovimentosHoje { get; set; }

        /// Últimos movimentos registados, ordenados por data desc.
        public IEnumerable<UltimoMovimentoItem> UltimosMovimentos { get; set; }
            = Array.Empty<UltimoMovimentoItem>();

        /// Resumo das obras ativas para o card da direita.
        public IEnumerable<ObraResumoItem> ObrasResumo { get; set; }
            = Array.Empty<ObraResumoItem>();
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

    /// Item usado no card "Obras ativas" (parte de baixo, direita).
    public sealed class ObraResumoItem
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        /// Número total de movimentos de material associados à obra.
        public int Movimentos { get; set; }

        /// Quantidade de materiais distintos usados na obra.
        public int MateriaisDistintos { get; set; }
    }
}