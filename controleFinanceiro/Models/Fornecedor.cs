using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro.Models
{
    public class Fornecedor
    {
        [Key] // <-- garante que o EF reconheça a chave primária
        public int FornecedorId { get; set; }

        [Required]
        public string Nome { get; set; }

        public string NumeroContato { get; set; }

        public int UsuarioId { get; set; }

        public DateTime DataCadastro { get; set; }

        public ICollection<CompraFornecedor> Compras { get; set; }
    }
}
