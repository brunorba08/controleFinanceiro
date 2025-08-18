using ControleFinanceiro.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Busca o usuário pelo ID da sessão
            var usuario = _context.Usuarios.Find(usuarioId.Value);
            if (usuario == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.UsuarioLogado = usuario.Nome;
            return View();
        }
    }
}
