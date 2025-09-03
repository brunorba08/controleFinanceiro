using ControleFinanceiro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace ControleFinanceiro.Controllers
{
    // Herda do BaseController que já recebe IHttpContextAccessor
    public class AccountController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Construtor agora também recebe o DbContext e IHttpContextAccessor
        public AccountController(AppDbContext context, IHttpContextAccessor accessor) : base(accessor)
        {
            _context = context;
            _httpContextAccessor = accessor;
        }

        // ---------- LOGOUT ----------
        public IActionResult Logout()
        {
            // Limpa toda a sessão
            _httpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        // ---------- LOGIN ----------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                ViewBag.Erro = "Preencha todos os campos";
                return View();
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario != null)
            {
                var passwordHasher = new PasswordHasher<Usuario>();
                var result = passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senha);

                if (result == PasswordVerificationResult.Success)
                {
                    _httpContextAccessor.HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
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

                TempData["Sucesso"] = "Usuario cadastrado com sucesso!";
                return RedirectToAction("Register");
            }

            return View(usuario);
        }


    }
}
