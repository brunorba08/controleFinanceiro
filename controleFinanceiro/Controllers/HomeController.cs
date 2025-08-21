using System.Diagnostics;
using controleFinanceiro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace controleFinanceiro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            // pega o usuário logado da sessão
            ViewBag.UsuarioLogado = _httpContextAccessor.HttpContext?.Session.GetString("UsuarioLogado");
            return View();
        }

        public IActionResult Privacy()
        {
            ViewBag.UsuarioLogado = _httpContextAccessor.HttpContext?.Session.GetString("UsuarioLogado");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewBag.UsuarioLogado = _httpContextAccessor.HttpContext?.Session.GetString("UsuarioLogado");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
