using ControleFinanceiro.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ControleFinanceiro.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string Email, string Senha)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == Email && u.Senha == Senha);

            if (usuario != null)
            {
                // Aqui no futuro pode colocar autenticação de verdade
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Erro = "Usuário ou senha inválidos.";
            return View();
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(usuario);
        }
    }
}
