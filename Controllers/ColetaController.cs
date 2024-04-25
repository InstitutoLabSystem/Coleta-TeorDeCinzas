using Coleta_TeorDeCinzas.Data;
using Coleta_TeorDeCinzas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
            var model = new ColetaViewModel();
            model.oColeta = obterColeta(os, orcamento);
            model.oInstrumento = ObterInstrumentos();
            model.oInstrumentoInformacao = ObterInstrumentosInformacao(os, orcamento);


            var usuario_sessao = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.usuario_sessao = usuario_sessao;
            ViewBag.os = os;
            ViewBag.orcamento = orcamento;

            return View(model);

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
        private List<InstrumentosModel> ObterInstrumentos()
        {
            var instrumentos = _quimicoContext.instrumentos_teor_cinzas.ToList();
            return instrumentos;
        }
        private List<InformacaoInstrumentos> ObterInstrumentosInformacao(string os, string orcamento)
        {
            var instrumentosInformacao = _quimicoContext.instrumentos_teor_cinza_informacao.
                Where(x => x.os == os && x.orcamento == orcamento).ToList();
            return instrumentosInformacao;
        }

        private ColetaModel obterColeta(string os, string orcamento)
        {

            var dados = _quimicoContext.coleta_teor_cinzas.
                Where(x => x.os == os && x.orcamento == orcamento).FirstOrDefault();
            return dados;
        }
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
                    editarDados.auxiliar = salvar.auxiliar;

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

        public async Task<IActionResult> salvarColeta(string os, string orcamento, ColetaViewModel salvarDados)
        {
            try
            {
                //pegando a sessão do usuario
                var usuario_sessao = User.FindFirstValue(ClaimTypes.Name);
                var dados = _quimicoContext.coleta_teor_cinzas.Where(x => x.os == os && x.orcamento == orcamento).FirstOrDefault();

                if (dados == null)
                {
                    //manipulando os resultados aqui.
                    salvarDados.oColeta.amostra_quatro = (float)Math.Round(((salvarDados.oColeta.amostra_tres - salvarDados.oColeta.amostra_um) / (salvarDados.oColeta.amostra_dois - salvarDados.oColeta.amostra_um) * 100), 2);
                    salvarDados.oColeta.duplicata_quatro = (float)Math.Round(((salvarDados.oColeta.duplicata_tres - salvarDados.oColeta.duplicata_um) / (salvarDados.oColeta.duplicata_dois - salvarDados.oColeta.duplicata_um) * 100), 2);
                    float? maior = null;

                    if (salvarDados.oColeta.amostra_quatro > salvarDados.oColeta.duplicata_quatro)
                    {
                        maior = salvarDados.oColeta.amostra_quatro;
                    }
                    else
                    {
                        maior = salvarDados.oColeta.duplicata_quatro;
                    }
                    //verificando o resultado.
                    if (maior >= 0.33)
                    {
                        salvarDados.oColeta.resultado = maior.ToString();
                    }
                    else
                    {
                        salvarDados.oColeta.resultado = "< LQ";
                    }

                    //salvando os dados da coleta.
                    var salvarColeta = new ColetaModel
                    {
                        os = os,
                        orcamento = orcamento,
                        parametro_enc = salvarDados.oColeta.parametro_enc,
                        parametro_enc_dois = salvarDados.oColeta.parametro_enc_dois,
                        amostra_um = salvarDados.oColeta.amostra_um,
                        amostra_dois = salvarDados.oColeta.amostra_dois,
                        amostra_tres = salvarDados.oColeta.amostra_tres,
                        duplicata_um = salvarDados.oColeta.duplicata_um,
                        duplicata_dois = salvarDados.oColeta.duplicata_dois,
                        duplicata_tres = salvarDados.oColeta.duplicata_tres,
                        amostra_quatro = salvarDados.oColeta.amostra_quatro,
                        duplicata_quatro = salvarDados.oColeta.duplicata_quatro,
                        resultado = salvarDados.oColeta.resultado,
                        observacao = salvarDados.oColeta.observacao,
                        conformidade = salvarDados.oColeta.conformidade,
                        decisao = salvarDados.oColeta.decisao,
                        executador = usuario_sessao
                    };
                    //SALVANO COLETA NO BANCO
                    _quimicoContext.coleta_teor_cinzas.Add(salvarColeta);

                    //TRABALHANDO COM A LISTA DE INSTRUMENTOS PARA SALVAR.
                    for (int i = 0; i < salvarDados.oInstrumento.Count; i++)
                    {
                        if (salvarDados.oInstrumento[i].validade == null)
                        {
                            salvarDados.oInstrumento[i].validade = DateOnly.Parse("0001-01-01");
                        }
                        //salvando os instrumentos, na tabela                    
                        var salvarInstrumentos = new InformacaoInstrumentos
                        {
                            os = os,
                            orcamento = orcamento,
                            codigo = salvarDados.oInstrumento[i].codigo,
                            descricao = salvarDados.oInstrumento[i].descricao,
                            validade = (DateOnly)salvarDados.oInstrumento[i].validade,
                        };

                        //SALVAR O ARRAY DO INSTRUMENTOS NO BANCO
                        _quimicoContext.instrumentos_teor_cinza_informacao.Add(salvarInstrumentos);
                    }

                    await _quimicoContext.SaveChangesAsync();
                    TempData["Mensagem"] = "Dados salvo com sucesso";
                    return RedirectToAction("Coleta", "Coleta", new { os, orcamento });
                }
                else
                {

                    //recebendo os novos valores para editar.
                    dados.parametro_enc = salvarDados.oColeta.parametro_enc;
                    dados.parametro_enc_dois = salvarDados.oColeta.parametro_enc_dois;
                    dados.amostra_um = salvarDados.oColeta.amostra_um;
                    dados.amostra_dois = salvarDados.oColeta.amostra_dois;
                    dados.amostra_tres = salvarDados.oColeta.amostra_tres;
                    dados.duplicata_um = salvarDados.oColeta.duplicata_um;
                    dados.duplicata_dois = salvarDados.oColeta.duplicata_dois;
                    dados.duplicata_tres = salvarDados.oColeta.duplicata_tres;
                    float? maior = null;

                    //realizando calculo de conta.
                    dados.amostra_quatro = (float)Math.Round((((dados.amostra_tres - dados.amostra_um) / (dados.amostra_dois - dados.amostra_um)) * 100), 2);
                    dados.duplicata_quatro = (float)Math.Round((((dados.duplicata_tres - dados.duplicata_um) / (dados.duplicata_dois - dados.duplicata_um)) * 100), 2);

                    //pegando o maior numero
                    if (dados.amostra_quatro > dados.duplicata_quatro)
                    {
                        maior = dados.amostra_quatro;
                    }
                    else
                    {
                        maior = dados.duplicata_quatro;
                    }

                    //verificando se o valor esta correto.
                    if (maior >= 0.33)
                    {
                        dados.resultado = maior.ToString();
                    }
                    else
                    {
                        dados.resultado = "< LQ";
                    }

                    //recebendo os novos valores para editar.
                    dados.observacao = salvarDados.oColeta.observacao;
                    dados.conformidade = salvarDados.oColeta.conformidade;
                    dados.decisao = salvarDados.oColeta.decisao;

                    _quimicoContext.coleta_teor_cinzas.Update(dados);
                    await _quimicoContext.SaveChangesAsync();

                    TempData["Mensagem"] = "Dados editado com sucesso";
                    return RedirectToAction("Coleta", "Coleta", new { os, orcamento });
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
