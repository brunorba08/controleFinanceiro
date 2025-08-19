using ControleFinanceiro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace ControleFinanceiro.Controllers
{
    public class TransacaoController : Controller
    {
        private readonly AppDbContext _context;

        public TransacaoController(AppDbContext context)
        {
            _context = context;
        }

        // Listar transações com filtro
        public IActionResult Index(DateTime? dataInicial, DateTime? dataFinal, string filtroPeriodo)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            var transacoes = _context.Transacoes
                                     .Where(t => t.UsuarioId == usuarioId)
                                     .AsQueryable();

            // Filtra por datas
            if (dataInicial.HasValue)
                transacoes = transacoes.Where(t => t.Data >= dataInicial.Value);

            if (dataFinal.HasValue)
                transacoes = transacoes.Where(t => t.Data <= dataFinal.Value.AddDays(1).AddTicks(-1));

            // Filtra por período específico
            if (!string.IsNullOrEmpty(filtroPeriodo))
            {
                var hoje = DateTime.Today;
                if (filtroPeriodo == "dia")
                    transacoes = transacoes.Where(t => t.Data.Date == hoje);
                else if (filtroPeriodo == "mes")
                    transacoes = transacoes.Where(t => t.Data.Month == hoje.Month && t.Data.Year == hoje.Year);
                else if (filtroPeriodo == "ano")
                    transacoes = transacoes.Where(t => t.Data.Year == hoje.Year);
            }

            var lista = transacoes.OrderByDescending(t => t.Data).ToList();
            ViewBag.UsuarioId = usuarioId;

            return View(lista);
        }

        // Formulário para adicionar
        [HttpGet]
        public IActionResult Adicionar()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            ViewBag.UsuarioId = usuarioId;
            return View();
        }

        // Salvar transação
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Adicionar(Transacao transacao)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                TempData["Erro"] = "Preencha todos os campos obrigatórios.";
                return View(transacao);
            }

            if (transacao.Data == default)
                transacao.Data = DateTime.Now;

            transacao.UsuarioId = usuarioId.Value;
            _context.Transacoes.Add(transacao);
            _context.SaveChanges();

            TempData["Sucesso"] = "Transação adicionada com sucesso!";
            return RedirectToAction("Index");
        }

        // GET: Transacao/Editar/5
        [HttpGet]
        public IActionResult Editar(int id)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            var transacao = _context.Transacoes.FirstOrDefault(t => t.Id == id && t.UsuarioId == usuarioId);
            if (transacao == null)
                return NotFound();

            return View(transacao);
        }

        // POST: Transacao/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Transacao model)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var transacao = _context.Transacoes.FirstOrDefault(t => t.Id == model.Id && t.UsuarioId == usuarioId);
                if (transacao == null)
                    return NotFound();

                transacao.Descricao = model.Descricao;
                transacao.Valor = model.Valor;
                transacao.Tipo = model.Tipo;
                transacao.Data = model.Data;
                transacao.FormaPagamento = model.FormaPagamento; // ✅ mantém campo atualizado

                _context.SaveChanges();

                TempData["Sucesso"] = "Transação editada com sucesso!";
                return RedirectToAction("Index");
            }

            TempData["Erro"] = "Erro ao editar a transação.";
            return View(model);
        }

        // POST: Transacao/Excluir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(int id)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            var transacao = _context.Transacoes.FirstOrDefault(t => t.Id == id && t.UsuarioId == usuarioId);
            if (transacao == null)
            {
                TempData["Erro"] = "Transação não encontrada.";
                return RedirectToAction("Index");
            }

            _context.Transacoes.Remove(transacao);
            _context.SaveChanges();

            TempData["Sucesso"] = "Transação excluída com sucesso!";
            return RedirectToAction("Index");
        }
    }
}
