﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.SGP.Api.Filtros;
using SME.SGP.Aplicacao;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Api.Controllers
{
    [Route("api/v1/professores")]
    [ApiController]
    [ValidaDto]
    [Authorize("Bearer")]
    public class ProfessorController : ControllerBase
    {
        private readonly IConsultasProfessor consultasProfessor;

        public ProfessorController(IConsultasProfessor consultasProfessor)
        {
            this.consultasProfessor = consultasProfessor;
        }

        [HttpGet("eventos/matriculas")]
        public async Task<IActionResult> EventosMatricula([FromServices] IServicoEventoMatricula eventos)
        {
            eventos.ExecutaCargaEventos();
            return Ok();
        }

        [HttpGet]
        [Route("{codigoRf}/turmas")]
        [ProducesResponseType(typeof(IEnumerable<ProfessorTurmaDto>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public IActionResult Get(string codigoRf)
        {
            return Ok(consultasProfessor.Listar(codigoRf));
        }

        [HttpGet("{codigoRF}/turmas/{codigoTurma}/disciplinas/")]
        [ProducesResponseType(typeof(IEnumerable<DisciplinaDto>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> Get(string codigoTurma, string codigoRF, [FromQuery] bool turmaPrograma, [FromServices]IConsultasDisciplina consultasDisciplina)
        {
            return Ok(await consultasDisciplina.ObterDisciplinasPorProfessorETurma(codigoTurma, turmaPrograma));
        }

        [HttpGet("{codigoRF}/escolas/{codigoEscola}/turmas/anos-letivos/{anoLetivo}")]
        [ProducesResponseType(typeof(IEnumerable<DisciplinaDto>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> Get(string codigoRF, string codigoEscola, int anoLetivo, [FromServices]IConsultasProfessor consultasProfessor)
        {
            return Ok(await consultasProfessor.ObterTurmasAtribuidasAoProfessorPorEscolaEAnoLetivo(codigoRF, codigoEscola, anoLetivo));
        }

        [HttpGet("turmas/{codigoTurma}/disciplinas/planejamento")]
        [ProducesResponseType(typeof(IEnumerable<DisciplinaDto>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        [Permissao(Permissao.PA_I, Permissao.PA_A, Permissao.PA_C, Policy = "Bearer")]
        public async Task<IActionResult> ObterDisciplinasParaPlanejamento(long codigoTurma, [FromQuery]FiltroDisciplinaPlanejamentoDto filtroDisciplinaPlanejamentoDto, [FromServices]IConsultasDisciplina consultasDisciplina)
        {
            filtroDisciplinaPlanejamentoDto.CodigoTurma = codigoTurma;

            return Ok(await consultasDisciplina.ObterDisciplinasParaPlanejamento(filtroDisciplinaPlanejamentoDto));
        }

        [HttpGet("{codigoRF}/resumo/{anoLetivo}/{incluirEmei}")]
        [ProducesResponseType(typeof(ProfessorResumoDto), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> Resumo(string codigoRF, int anoLetivo, bool incluirEmei)
        {
            var retorno = await consultasProfessor.ObterResumoPorRFAnoLetivo(codigoRF, anoLetivo, incluirEmei);

            return Ok(retorno);
        }

        [HttpGet("{codigoRF}/resumo/{anoLetivo}")]
        [ProducesResponseType(typeof(ProfessorResumoDto), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> Resumo(string codigoRF, int anoLetivo)
        {
            var retorno = await consultasProfessor.ObterResumoPorRFAnoLetivo(codigoRF, anoLetivo);

            return Ok(retorno);
        }

        [HttpGet("{anoLetivo}/autocomplete/{dreId}")]
        [ProducesResponseType(typeof(IEnumerable<ProfessorResumoDto>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> ResumoAutoComplete(int anoLetivo, string dreId, string nomeProfessor)
        {
            var retorno = await consultasProfessor.ObterResumoAutoComplete(anoLetivo, dreId, nomeProfessor);

            if (retorno == null)
                return NoContent();

            return Ok(retorno);
        }

        [HttpGet("{anoLetivo}/autocomplete/{dreId}/{incluirEmei}")]
        [ProducesResponseType(typeof(IEnumerable<ProfessorResumoDto>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> ResumoAutoComplete(int anoLetivo, string dreId, string nomeProfessor, bool incluirEmei)
        {
            var retorno = await consultasProfessor.ObterResumoAutoComplete(anoLetivo, dreId, nomeProfessor, incluirEmei);

            if (retorno == null)
                return NoContent();

            return Ok(retorno);
        }
    }
}