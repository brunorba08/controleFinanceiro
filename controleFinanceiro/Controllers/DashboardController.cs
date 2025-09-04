using ControleFinanceiro.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ControleFinanceiro.Controllers
{
    // Herdando BaseController para ter acesso à sessão via IHttpContextAccessor
    public class DashboardController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardController(AppDbContext context, IHttpContextAccessor accessor) : base(accessor)
        {
            _context = context;
            _httpContextAccessor = accessor;
        }

        public IActionResult Index(string tipoFiltro, string dataFiltro)
        {
            int? usuarioId = _httpContextAccessor.HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            var usuario = _context.Usuarios.Find(usuarioId.Value);
            if (usuario == null)
                return RedirectToAction("Login", "Account");

            ViewBag.UsuarioLogado = usuario.Nome;

            var transacoes = _context.Transacoes
                                     .Where(t => t.UsuarioId == usuarioId.Value)
                                     .AsQueryable();

            string filtroDescricao = "";
            List<Transacao> listaFiltrada = new List<Transacao>();
            Dictionary<string, decimal> dadosGanhos = new Dictionary<string, decimal>();
            Dictionary<string, decimal> dadosDespesas = new Dictionary<string, decimal>();

            if (string.IsNullOrEmpty(tipoFiltro) || tipoFiltro == "dia")
            {
                DateTime diaFiltro = !string.IsNullOrEmpty(dataFiltro)
                                     ? DateTime.Parse(dataFiltro)
                                     : DateTime.Today;

                listaFiltrada = transacoes.Where(t => t.Data.Date == diaFiltro.Date)
                                          .OrderByDescending(t => t.Data)
                                          .ToList();

                dadosGanhos[diaFiltro.ToString("dd/MM/yyyy")] = listaFiltrada.Where(t => t.Tipo == "Ganho").Sum(t => t.Valor);
                dadosDespesas[diaFiltro.ToString("dd/MM/yyyy")] = listaFiltrada.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);

                filtroDescricao = $"Transações do dia {diaFiltro:dd/MM/yyyy}";
            }
            else if (tipoFiltro == "mes")
            {
                DateTime mesFiltro = !string.IsNullOrEmpty(dataFiltro)
                                     ? DateTime.Parse(dataFiltro + "-01") 
                                     : DateTime.Today;

                int mes = mesFiltro.Month;
                int ano = mesFiltro.Year;

                listaFiltrada = transacoes.Where(t => t.Data.Month == mes && t.Data.Year == ano)
                                          .OrderBy(t => t.Data)
                                          .ToList();

                var grupoDias = listaFiltrada.GroupBy(t => t.Data.Date).OrderBy(g => g.Key);

                foreach (var g in grupoDias)
                {
                    string label = g.Key.ToString("dd/MM/yyyy");
                    dadosGanhos[label] = g.Where(t => t.Tipo == "Ganho").Sum(t => t.Valor);
                    dadosDespesas[label] = g.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);
                }

                filtroDescricao = $"Transações do mês {mes:00}/{ano}";
            }
            else if (tipoFiltro == "ano")
            {
                int anoFiltro = !string.IsNullOrEmpty(dataFiltro) ? int.Parse(dataFiltro) : DateTime.Today.Year;

                listaFiltrada = transacoes.Where(t => t.Data.Year == anoFiltro)
                                          .OrderBy(t => t.Data)
                                          .ToList();

                var grupoMeses = listaFiltrada.GroupBy(t => t.Data.Month)
                                              .OrderBy(g => g.Key);

                foreach (var g in grupoMeses)
                {
                    string label = new DateTime(anoFiltro, g.Key, 1).ToString("MM/yyyy");
                    dadosGanhos[label] = g.Where(t => t.Tipo == "Ganho").Sum(t => t.Valor);
                    dadosDespesas[label] = g.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);
                }

                filtroDescricao = $"Transações do ano {anoFiltro}";
            }

            decimal totalGanhos = listaFiltrada.Where(t => t.Tipo == "Ganho").Sum(t => t.Valor);
            decimal totalDespesas = listaFiltrada.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);

            var viewModel = new DashboardViewModel
            {
                TotalGanhos = totalGanhos,
                TotalDespesas = totalDespesas,
                Transacoes = listaFiltrada,
                LabelsGrafico = dadosGanhos.Keys.ToList(),
                DadosGanhos = dadosGanhos.Values.ToList(),
                DadosDespesas = dadosDespesas.Values.ToList(),
                LabelsCategorias = new List<string> { "Ganhos", "Despesas" },
                DadosCategorias = new List<decimal> { totalGanhos, totalDespesas }
            };

            ViewBag.Filtro = filtroDescricao;

            return View(viewModel);
        }
    }
}
