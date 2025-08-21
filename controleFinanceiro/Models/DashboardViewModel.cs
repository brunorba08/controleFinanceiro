namespace ControleFinanceiro.Models
{
    public class DashboardViewModel
    {
        public decimal TotalDespesas { get; set; }
        public decimal TotalGanhos { get; set; }
        public decimal Saldo => TotalGanhos - TotalDespesas;

        public List<Transacao> Transacoes { get; set; }

        // Gráficos
        public List<string> LabelsGrafico { get; set; }
        public List<decimal> DadosDespesas { get; set; }
        public List<decimal> DadosGanhos { get; set; }
        public List<string> LabelsCategorias { get; set; }
        public List<decimal> DadosCategorias { get; set; }
    }
}
