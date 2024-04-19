using Coleta_TeorDeCinzas.Data;
using Coleta_TeorDeCinzas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Coleta_TeorDeCinzas.Controllers
{
    [Authorize]
    public class ColetaController : Controller
    {
        private readonly ILogger<ColetaController> _logger;
        private readonly BancoContext _bancoContext;
        private readonly QuimicoContext _quimicoContext;

        public ColetaController(ILogger<ColetaController> logger, BancoContext bancoContext, QuimicoContext quimicoContext)
        {
            _logger = logger;
            _bancoContext = bancoContext;
            _quimicoContext = quimicoContext;
        }

        public IActionResult Coleta(string os, string orcamento)
        {
            var usuario_sessao = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.usuario_sessao = usuario_sessao;
            ViewBag.os = os;
            ViewBag.orcamento = orcamento;
            return View();

        }
        public IActionResult InicioColeta(string os, string orcamento)
        {
            var usuario_sessao = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.usuario_sessao = usuario_sessao;
            ViewBag.os = os;
            ViewBag.orcamento = orcamento;
            return View();
        }
        public IActionResult editarInicio(string os, string orcamento)
        {
            var dados = _quimicoContext.registro_teor_cinzas.Where(x => x.os == os && x.orcamento == orcamento).FirstOrDefault();
            var usuario_sessao = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.usuario_sessao = usuario_sessao;
            ViewBag.os = os;
            ViewBag.orcamento = orcamento;

            return View("EditarInicio", dados);
        }

        [HttpPost]
        public async Task<IActionResult> salvarInico(string os, string orcamento, InicioColetaModel salvarInicio)
        {
            try
            {
                var usuario_sessao = User.FindFirstValue(ClaimTypes.Name);
                salvarInicio.executador = usuario_sessao;

                _quimicoContext.registro_teor_cinzas.Add(salvarInicio);
                await _quimicoContext.SaveChangesAsync();

                TempData["Mensagem"] = "Dados salvo com sucesso";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário", ex.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> editarInicio(string os, string orcamento, InicioColetaModel salvar)
        {
            try
            {
                var editarDados = _quimicoContext.registro_teor_cinzas.Where(x => x.os == os && x.orcamento == orcamento).FirstOrDefault();

                if (editarDados != null)
                {
                    editarDados.data_ini = salvar.data_ini;
                    editarDados.data_term = salvar.data_term;
                    editarDados.minimo = salvar.minimo;
                    editarDados.maximo = salvar.maximo;

                    _quimicoContext.registro_teor_cinzas.Update(editarDados);
                    await _quimicoContext.SaveChangesAsync();

                    TempData["Mensagem"] = "Dados editado com sucesso";
                    return RedirectToAction("EditarInicio", "Coleta", new { os, orcamento });
                }
                else
                {
                    TempData["Mensagem"] = "Erro ao editar";
                    return RedirectToAction("EditarInicio", "Coleta", new { os, orcamento });
                }

               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário", ex.Message);
                throw;
            }
        }
    }
}
