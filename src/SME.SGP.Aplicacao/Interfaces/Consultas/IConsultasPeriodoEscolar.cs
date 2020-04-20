﻿using SME.SGP.Dominio;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public interface IConsultasPeriodoEscolar
    {
        PeriodoEscolar ObterPeriodoEscolarPorData(long tipoCalendarioId, DateTime dataPeriodo);
        PeriodoEscolarListaDto ObterPorTipoCalendario(long tipoCalendarioId);
        DateTime ObterFimPeriodoRecorrencia(long tipoCalendarioId, DateTime inicioRecorrencia, RecorrenciaAula recorrencia);

        int ObterBimestre(DateTime data, Modalidade modalidade);
        Task<IEnumerable<PeriodoEscolar>> ObterPeriodosEmAberto(long ueId, Modalidade modalidadeCodigo, int anoLetivo);
        Task<PeriodoEscolarDto> ObterUltimoPeriodoAsync(int anoLetivo, ModalidadeTipoCalendario modalidade, int semestre);
        PeriodoEscolar ObterPeriodoPorModalidade(Modalidade modalidade, DateTime data);
        PeriodoEscolar ObterPeriodoAtualPorModalidade(Modalidade modalidade);
        PeriodoEscolar ObterPeriodoPorData(IEnumerable<PeriodoEscolar> periodosEscolares, DateTime data);
        PeriodoEscolar ObterUltimoPeriodoPorData(IEnumerable<PeriodoEscolar> periodosEscolares, DateTime data);
        IEnumerable<PeriodoEscolar> ObterPeriodosEscolares(long tipoCalendarioId);
    }
}