using System;
using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro.Models
{
    public class Transacao
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        [Required]
        public decimal Valor { get; set; }

        [Required]
        [StringLength(200)]
        public string Descricao { get; set; }

        [Required]
        public string Tipo { get; set; } // Ganho ou Despesa

        public DateTime Data { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? FormaPagamento { get; set; } // Dinheiro, Cartão, PIX, Boleto
    }
}
