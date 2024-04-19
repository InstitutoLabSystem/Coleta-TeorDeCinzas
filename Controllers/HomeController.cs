using Coleta_TeorDeCinzas.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Coleta_TeorDeCinzas.Data;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data.Entity.Core.Mapping;
using System.Security.Claims;

namespace Coleta_TeorDeCinzas.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BancoContext _bancoContext;
        private readonly QuimicoContext _qContext;



        public HomeController(ILogger<HomeController> logger, BancoContext bancoContext, QuimicoContext qContext)
        {
            _logger = logger;
            _bancoContext = bancoContext;
            _qContext = qContext;
        }
        public async Task<IActionResult> LogOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Acess");
        }

        public IActionResult Index()
        {
            var usuario_sessao = User.FindFirstValue(ClaimTypes.Name);
            return View();
        }

        public async Task<IActionResult> BuscarOrcamento(string OS)
        {
            
            var dados = (from lab in _bancoContext.ordemservico_laboratorio
                         where lab.OS == OS && lab.Andamento != "ENVIADO" && lab.Laboratorio == "Teor de Cinzas"
                         select new OrdemServicoLaboratorio
                         {
                             
                             orcamento = lab.orcamento,
                             OS = OS,
                             Laboratorio = lab.Laboratorio,
                             Status = lab.Status,
                         }).FirstOrDefault();

            if(dados == null)
            {
                TempData["Mensagem"] = "Não foi encontrado essa ordem de serviço";
                return View("Index");
            }

            var ExisteOs = _qContext.registro_teor_cinzas.Where(x => x.os == OS && x.orcamento == dados.orcamento).FirstOrDefault();
            if (ExisteOs == null)
            {
                return RedirectToAction("InicioColeta", "Coleta", new { OS, dados.orcamento });
            }

            ViewBag.os = OS;
            ViewBag.orcamento = dados.orcamento;
            return View("Index",dados);
        }
    }
}
