using ControleFinanceiro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Controllers
{
    public class FornecedorController : Controller
    {
        private readonly AppDbContext _context;

        public FornecedorController(AppDbContext context)
        {
            _context = context;
        }

        // Tela inicial de fornecedores
        public IActionResult Index()
        {
            int usuarioId = (int)HttpContext.Session.GetInt32("UsuarioId");
            var fornecedores = _context.Fornecedores
                                       .Where(f => f.UsuarioId == usuarioId)
                                       .ToList();
            return View(fornecedores);
        }

        // Adicionar fornecedor
        public IActionResult Adicionar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Adicionar(Fornecedor fornecedor)
        {
            fornecedor.UsuarioId = (int)HttpContext.Session.GetInt32("UsuarioId");
            fornecedor.DataCadastro = DateTime.Now;
            _context.Fornecedores.Add(fornecedor);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Detalhes do fornecedor + compras
        public IActionResult Detalhes(int id)
        {
            var fornecedor = _context.Fornecedores
                                     .Include(f => f.Compras)
                                     .FirstOrDefault(f => f.FornecedorId == id);
            return View(fornecedor);
        }

        // Adicionar compra ao fornecedor
        public IActionResult AdicionarCompra(int fornecedorId)
        {
            var compra = new CompraFornecedor { FornecedorId = fornecedorId, Data = DateTime.Now };
            return View(compra);
        }

        [HttpPost]
        public IActionResult AdicionarCompra(CompraFornecedor compra)
        {
            compra.UsuarioId = (int)HttpContext.Session.GetInt32("UsuarioId");
            _context.ComprasFornecedor.Add(compra);
            _context.SaveChanges();
            return RedirectToAction("Detalhes", new { id = compra.FornecedorId });
        }
    }
}
