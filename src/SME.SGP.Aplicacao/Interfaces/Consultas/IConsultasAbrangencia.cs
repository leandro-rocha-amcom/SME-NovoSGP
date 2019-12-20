﻿using SME.SGP.Dominio;
using SME.SGP.Dto;
using SME.SGP.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public interface IConsultasAbrangencia
    {
        Task<IEnumerable<AbrangenciaFiltroRetorno>> ObterAbrangenciaPorfiltro(string texto, bool consideraHistorico);
        Task<AbrangenciaFiltroRetorno> ObterAbrangenciaTurma(string turma);

        Task<IEnumerable<AbrangenciaDreRetorno>> ObterDres(Modalidade? modalidade, int periodo = 0, bool consideraHistorico = false);

        Task<IEnumerable<EnumeradoRetornoDto>> ObterModalidades(int anoLetivo, bool consideraHistorico);

        Task<IEnumerable<int>> ObterSemestres(Modalidade modalidade, bool consideraHistorico);

        Task<IEnumerable<AbrangenciaTurmaRetorno>> ObterTurmas(string codigoUe, Modalidade modalidade, int periodo = 0, bool consideraHistorico = false);

        Task<IEnumerable<AbrangenciaUeRetorno>> ObterUes(string codigoDre, Modalidade? modalidade, int periodo = 0, bool consideraHistorico = false);
        Task<IEnumerable<int>> ObterAnosLetivos(bool consideraHistorico);
    }
}