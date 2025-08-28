using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro.Models
{
    public class Fornecedor
    {
        [Key]
        public int FornecedorId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O contato é obrigatório.")]
        [RegularExpression(@"^\d{1,11}$", ErrorMessage = "Digite somente números, até 11 dígitos.")]
        public string NumeroContato { get; set; }

        public int UsuarioId { get; set; }

        public DateTime DataCadastro { get; set; }

        // Inicializando a coleção para evitar erro de "required"
        public ICollection<CompraFornecedor> Compras { get; set; } = new List<CompraFornecedor>();
    }
}
