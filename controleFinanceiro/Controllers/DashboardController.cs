using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
