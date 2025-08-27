using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ControleFinanceiro.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<CompraFornecedor> ComprasFornecedor { get; set; }

    }
}
