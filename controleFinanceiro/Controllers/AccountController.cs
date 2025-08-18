using ControleFinanceiro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // Para PasswordHasher

namespace ControleFinanceiro.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // limpa a sessão
            return RedirectToAction("Login", "Account");
        }

        // ---------- LOGIN ----------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                ViewBag.Erro = "Preencha todos os campos";
                return View();
            }

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);

            if (usuario != null)
            {
                var passwordHasher = new PasswordHasher<Usuario>();
                var result = passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senha);

                if (result == PasswordVerificationResult.Success)
                {
                    // 👉 Salva o usuário logado na sessão
                    HttpContext.Session.SetInt32("UsuarioId", usuario.Id);

                    // Redireciona para o dashboard
                    return RedirectToAction("Index", "Dashboard");
                }
            }

            ViewBag.Erro = "Usuário ou senha inválidos";
            return View();
        }

        // ---------- REGISTER ----------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Criptografa a senha antes de salvar
                var passwordHasher = new PasswordHasher<Usuario>();
                usuario.Senha = passwordHasher.HashPassword(usuario, usuario.Senha);

                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                TempData["Sucesso"] = "Usuário cadastrado com sucesso!";
                return RedirectToAction("Register");
            }

            return View(usuario);
        }
    }
}
