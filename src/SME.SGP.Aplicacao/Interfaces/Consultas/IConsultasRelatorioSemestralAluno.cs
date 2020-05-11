﻿using SME.SGP.Dominio;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public interface IConsultasRelatorioSemestralAluno
    {
        Task<RelatorioSemestralAluno> ObterPorTurmaAlunoAsync(long relatorioSemestralId, string alunoCodigo);
        Task<IEnumerable<AlunoDadosBasicosDto>> ObterListaAlunosAsync(string turmaCodigo, int anoLetivo, int semestre);
        Task<IEnumerable<RelatorioSemestralAluno>> ObterRelatoriosAlunosPorTurmaAsync(long turmaId, int semestre);
    }
}
