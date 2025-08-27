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

        // ===========================
        // Tela inicial de fornecedores
        // ===========================
        public IActionResult Index()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            var fornecedores = _context.Fornecedores
                                       .Where(f => f.UsuarioId == usuarioId.Value)
                                       .ToList();
            return View(fornecedores);
        }

        // ===========================
        // Tela de adicionar fornecedor
        // ===========================
        public IActionResult Adicionar()
        {
            return View();
        }

        // POST: Adicionar fornecedor
        [HttpPost]
        public IActionResult Adicionar(Fornecedor fornecedor)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(fornecedor);

            fornecedor.UsuarioId = usuarioId.Value;
            fornecedor.DataCadastro = DateTime.Now;

            _context.Fornecedores.Add(fornecedor);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ===========================
        // Tela de detalhes do fornecedor + compras
        // ===========================
        [HttpGet("Fornecedor/Detalhes/{id}")]
        public IActionResult Detalhes(int id, DateTime? filtroData)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            var fornecedor = _context.Fornecedores
                                     .Include(f => f.Compras)
                                     .FirstOrDefault(f => f.FornecedorId == id && f.UsuarioId == usuarioId.Value);

            if (fornecedor == null)
                return NotFound();

            // Ordena do mais recente para o mais antigo
            fornecedor.Compras = fornecedor.Compras
                                           .OrderByDescending(c => c.Data)
                                           .ToList();

            // Aplica filtro se informado
            if (filtroData.HasValue)
            {
                fornecedor.Compras = fornecedor.Compras
                                               .Where(c => c.Data.Date == filtroData.Value.Date)
                                               .ToList();
            }

            return View(fornecedor);
        }

        // ===========================
        // Tela de adicionar compra
        // ===========================
        public IActionResult AdicionarCompra(int fornecedorId)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            var compra = new CompraFornecedor
            {
                FornecedorId = fornecedorId,
                Data = DateTime.Now
            };
            return View(compra);
        }

        // ===========================
        // POST: Adicionar compra sem foto
        // ===========================
        [HttpPost]
        [Route("Fornecedor/AdicionarCompraSemFoto")]
        public IActionResult AdicionarCompraSemFoto(CompraFornecedor compra)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            if (compra == null)
                return BadRequest("Dados da compra inválidos.");

            compra.UsuarioId = usuarioId.Value;

            _context.ComprasFornecedor.Add(compra);
            _context.SaveChanges();

            return RedirectToAction("Detalhes", new { id = compra.FornecedorId });
        }

        // ===========================
        // POST: Adicionar compra com foto
        // ===========================
        [HttpPost]
        [Route("Fornecedor/AdicionarCompraComFoto")]
        public IActionResult AdicionarCompraComFoto(CompraFornecedor compra, IFormFile? Foto)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            if (compra == null)
                return BadRequest("Dados da compra inválidos.");

            compra.UsuarioId = usuarioId.Value;

            if (Foto != null && Foto.Length > 0)
            {
                var pastaUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(pastaUploads))
                    Directory.CreateDirectory(pastaUploads);

                var nomeArquivo = $"{Guid.NewGuid()}_{Path.GetFileName(Foto.FileName)}";
                var caminhoCompleto = Path.Combine(pastaUploads, nomeArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    Foto.CopyTo(stream);
                }

                compra.FotoCaminho = nomeArquivo;
            }

            _context.ComprasFornecedor.Add(compra);
            _context.SaveChanges();

            return RedirectToAction("Detalhes", new { id = compra.FornecedorId });
        }
    }
}
