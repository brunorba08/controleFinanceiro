using controleFinanceiro.Models;
using ControleFinanceiro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControleFinanceiro.Controllers
{
    public class TransacaoController : BaseController
    {
        private readonly AppDbContext _context;

        public TransacaoController(AppDbContext context, IHttpContextAccessor accessor)
            : base(accessor)
        {
            _context = context;
        }

        // LISTAR
        public IActionResult Index(DateTime? dataInicial, DateTime? dataFinal, string filtroPeriodo)
        {
            int? usuarioId = HttpContextAtivo.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            ViewBag.UsuarioLogado = HttpContextAtivo.Session.GetString("UsuarioLogado");

            var transacoes = _context.Transacoes
                                     .Where(t => t.UsuarioId == usuarioId)
                                     .AsQueryable();

            if (dataInicial.HasValue)
                transacoes = transacoes.Where(t => t.Data >= dataInicial.Value);

            if (dataFinal.HasValue)
                transacoes = transacoes.Where(t => t.Data <= dataFinal.Value.AddDays(1).AddTicks(-1));

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

            // nunca manda null pra view
            return View(lista ?? new List<Transacao>());
        }

        // ADICIONAR
        [HttpGet]
        public IActionResult Adicionar()
        {
            int? usuarioId = HttpContextAtivo.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            ViewBag.UsuarioLogado = HttpContextAtivo.Session.GetString("UsuarioLogado");
            ViewBag.UsuarioId = usuarioId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Adicionar(Transacao transacao)
        {
            int? usuarioId = HttpContextAtivo.Session.GetInt32("UsuarioId");
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

        // EDITAR
        [HttpGet]
        public IActionResult Editar(int id)
        {
            int? usuarioId = HttpContextAtivo.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            ViewBag.UsuarioLogado = HttpContextAtivo.Session.GetString("UsuarioLogado");

            var transacao = _context.Transacoes.FirstOrDefault(t => t.Id == id && t.UsuarioId == usuarioId);
            if (transacao == null)
                return NotFound();

            return View(transacao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Transacao model)
        {
            int? usuarioId = HttpContextAtivo.Session.GetInt32("UsuarioId");
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
                transacao.FormaPagamento = model.FormaPagamento;

                _context.SaveChanges();

                TempData["Sucesso"] = "Transação editada com sucesso!";
                return RedirectToAction("Index");
            }

            TempData["Erro"] = "Erro ao editar a transação.";
            return View(model);
        }

        // EXCLUIR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(int id)
        {
            int? usuarioId = HttpContextAtivo.Session.GetInt32("UsuarioId");
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

        public IActionResult ResumoDiario(DateTime? dataInicial, DateTime? dataFinal)
        {
            // Se não passou datas, usa hoje
            var hoje = DateTime.Today;
            var inicio = dataInicial ?? hoje;
            var fim = dataFinal ?? hoje;

            // Busca transações do período
            var transacoes = _context.Transacoes
                .Where(t => t.Data.Date >= inicio && t.Data.Date <= fim)
                .OrderBy(t => t.Data)
                .ToList();

            // Passa datas para ViewBag (para inputs)
            ViewBag.DataInicial = inicio;
            ViewBag.DataFinal = fim;

            return View(transacoes);
        }


    }
}
