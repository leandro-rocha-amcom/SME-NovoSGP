﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.SGP.Api.Filtros;
using SME.SGP.Aplicacao;
using SME.SGP.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Api.Controllers
{
    [ApiController]
    [Route("api/v1/calendarios/tipos")]
    [ValidaDto]
    [Authorize("Bearer")]
    public class TipoCalendarioController : ControllerBase
    {
        private readonly IComandosTipoCalendario comandos;
        private readonly IConsultasTipoCalendario consultas;

        public TipoCalendarioController(IConsultasTipoCalendario consultas,
            IComandosTipoCalendario comandos)
        {
            this.consultas = consultas ?? throw new System.ArgumentNullException(nameof(consultas));
            this.comandos = comandos ?? throw new System.ArgumentNullException(nameof(comandos));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TipoCalendarioDto>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public IActionResult BuscarTodos()
        {
            return Ok(consultas.Listar());
        }

        [HttpGet]
        [ProducesResponseType(typeof(TipoCalendarioCompletoDto), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        [Route("{id}")]
        public IActionResult BuscarUm(long id)
        {
            return Ok(consultas.BuscarPorId(id));
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public IActionResult MarcarExcluidos([FromBody]long[] ids)
        {
            comandos.MarcarExcluidos(ids);
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> Salvar([FromBody]TipoCalendarioDto dto)
        {
            await comandos.Salvar(dto);
            return Ok();
        }
    }
}