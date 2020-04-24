﻿using SME.SGP.Dominio;
using SME.SGP.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public interface IConsultasFechamentoNota
    {
        Task<IEnumerable<NotaConceitoBimestreComponenteDto>> ObterNotasAlunoBimestre(long fechamentoTurmaId, string alunoCodigo);
        Task<IEnumerable<NotaConceitoBimestreComponenteDto>> ObterNotasAlunoAno(string turmaCodigo, string alunoCodigo);
    }
}
