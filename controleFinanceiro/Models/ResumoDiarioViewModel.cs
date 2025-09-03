using ControleFinanceiro.Models;
using System;
using System.Collections.Generic;

namespace controleFinanceiro.Models
{
    public class ResumoDiarioViewModel
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}
