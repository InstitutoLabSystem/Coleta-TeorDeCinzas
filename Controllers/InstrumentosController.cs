﻿using Coleta_TeorDeCinzas.Data;
using Coleta_TeorDeCinzas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.CompilerServices;

namespace Coleta_TeorDeCinzas.Controllers
{
    public class InstrumentosController : Controller
    {
        private readonly ILogger<InstrumentosController> _logger;
        private readonly QuimicoContext _quimicoContext;

        public InstrumentosController(ILogger<InstrumentosController> logger, QuimicoContext quimicoContext)
        {
            _logger = logger;
            _quimicoContext = quimicoContext;
        }

        public IActionResult Index()
        {
            var dados = _quimicoContext.instrumentos_teor_cinzas.Where(x=>x.ativo == 1).ToList();
            return View(dados);
        }

        public IActionResult InstrumentosEditar(int? Id)
        {
            var dados = _quimicoContext.instrumentos_teor_cinzas.Where(x => x.Id == Id).FirstOrDefault();
            return View(dados);
        }

        public IActionResult InstrumentosCriar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Criar(InstrumentosModel salvar)
        {
            if (salvar.codigo == null)
            {
                TempData["Mensagem"] = "o codigo não pode estar vazio";
            }

            _quimicoContext.instrumentos_teor_cinzas.Add(salvar);
            await _quimicoContext.SaveChangesAsync();

            TempData["Mensagem"] = "Dados salvo com sucesso";
            return RedirectToAction("instrumentosEditar", "Instrumentos", new { salvar.Id });
        }


        [HttpPost]
        public async Task<IActionResult> editarInstrumentos(int? Id, InstrumentosModel editar)
        {

            editar.ativo = 1;
            _quimicoContext.instrumentos_teor_cinzas.Update(editar);
            await _quimicoContext.SaveChangesAsync();

            TempData["Mensagem"] = "Dados editado com sucesso";
            return View("InstrumentosEditar", editar);
        }

        [HttpPost]
        public async Task<IActionResult> InstrumentosExcluir(int? Id, InstrumentosModel excluir)
        {
            try
            {
                //pegando o id selecionado.
                var dados = _quimicoContext.instrumentos_teor_cinzas
                    .Where(x => x.Id == Id).FirstOrDefault();
    
                //passando novo valor no ativo.
                dados.ativo = 0;

                _quimicoContext.instrumentos_teor_cinzas .Update(dados);
                await _quimicoContext.SaveChangesAsync();

                TempData["Mensagem"] = "Excluido com sucesso";
                return RedirectToAction("Index", "Instrumentos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário", ex.Message);
                throw;
            }
        }
    }
}
