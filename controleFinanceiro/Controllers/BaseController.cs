using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Propriedade de conveniência para acessar HttpContext
        protected HttpContext HttpContextAtivo => _httpContextAccessor.HttpContext;
    }
}
