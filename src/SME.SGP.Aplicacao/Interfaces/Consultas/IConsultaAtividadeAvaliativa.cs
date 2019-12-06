﻿using SME.SGP.Infra;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public interface IConsultaAtividadeAvaliativa
    {
        Task<PaginacaoResultadoDto<AtividadeAvaliativaCompletaDto>> ListarPaginado(FiltroAtividadeAvaliativaDto filtro);

        Task<AtividadeAvaliativaCompletaDto> ObterPorIdAsync(long id);
    }
}