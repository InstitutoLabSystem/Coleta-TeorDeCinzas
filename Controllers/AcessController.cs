using Coleta_TeorDeCinzas.Data;
using Coleta_TeorDeCinzas.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Coleta_TeorDeCinzas.Controllers
{
    public class AcessController : Controller
    {
        private readonly ILogger<AcessController> _logger;
        private readonly BancoContext _bancoContext;

        public AcessController(ILogger<AcessController> logger, BancoContext bancoContext)
        {
            _logger = logger;
            _bancoContext = bancoContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AcessModel.Usuario modelLogin)
        {
            try
            {
                if (string.IsNullOrEmpty(modelLogin.Nome_Usuario) || string.IsNullOrEmpty(modelLogin.Senha_Usuario))
                {
                    TempData["Mensagem"] = "Por favor, preencha o nome de usuário e senha.";
                    return View("Index", "Acess");
                }

                modelLogin.Nome_Usuario = modelLogin.Nome_Usuario.ToUpper();
                modelLogin.Senha_Usuario = modelLogin.Senha_Usuario.ToUpper();

                var pegarUsuario = _bancoContext.usuario
                                   .Where(x => x.Nome_Usuario == modelLogin.Nome_Usuario)
                                   .Select(x => new
                                   {
                                       x.Nome_Usuario,
                                       x.Senha_Usuario,
                                       x.cargo,
                                       x.setor,
                                       x.nomecompleto,
                                       x.laboratorio
                                   }).FirstOrDefault();

                if (pegarUsuario != null)
                {
                    if (pegarUsuario.Nome_Usuario == modelLogin.Nome_Usuario && pegarUsuario.Senha_Usuario == modelLogin.Senha_Usuario)
                    {
                        if(pegarUsuario.setor == "Químico" || pegarUsuario.setor == "Químico" || pegarUsuario.setor == "Colchão" || pegarUsuario.setor == "TI")
                        {
                            List<Claim> claims = new List<Claim>()
                                {
                                new Claim(ClaimTypes.NameIdentifier,modelLogin.Nome_Usuario),
                                new Claim(ClaimTypes.Name, pegarUsuario.nomecompleto),
                                new Claim("OtherProperties","Example Role")
                                };

                            ClaimsIdentity claimsIdenty = new ClaimsIdentity(claims,
                                CookieAuthenticationDefaults.AuthenticationScheme);

                            AuthenticationProperties properties = new AuthenticationProperties()
                            {
                                AllowRefresh = true,

                            };

                            TempData["Mensagem"] = "Logado Com Sucesso";
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdenty), properties);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        TempData["Mensagem"] = "Usuário não tem permissão";
                        return View("Index", "Acess");
                    }
                }
                else
                {
                    TempData["Mensagem"] = "Usuário Errado";
                    return View("Login", "Acess");
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário", ex.Message);
                throw;
            }

        }
    }
}
