using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleFinanceiro.Models
{
    public class CompraFornecedor
    {
        [Key]
        [Column("CompraId")]  // corresponde ao banco
        public int Id { get; set; }

        [Required]
        public int FornecedorId { get; set; }

        [ForeignKey("FornecedorId")]
        public Fornecedor Fornecedor { get; set; }

        [Required]
        public string Descricao { get; set; }

        [Required]
        public decimal Valor { get; set; }

        [Required]
        [Column("DataCompra")] // corresponde ao banco
        public DateTime Data { get; set; }

        [Column("Foto")] // aqui é o importante
        public string? FotoCaminho { get; set; }
        public int UsuarioId { get; set; }
    }

}
