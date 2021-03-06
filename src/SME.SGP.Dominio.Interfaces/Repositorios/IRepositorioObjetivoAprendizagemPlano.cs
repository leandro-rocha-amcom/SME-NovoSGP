﻿using SME.SGP.Infra;
using System.Collections.Generic;

namespace SME.SGP.Dominio.Interfaces
{
    public interface IRepositorioObjetivoAprendizagemPlano : IRepositorioBase<ObjetivoAprendizagemPlano>
    {
        IEnumerable<ComponenteCurricularSimplificadoDto> ObterDisciplinasDoBimestrePlanoAula(int ano, int bimestre, long turmaId, long componenteCurricularId);

        long ObterIdPorObjetivoAprendizagemJurema(long planoId, long objetivoAprendizagemJuremaId);

        IEnumerable<ObjetivoAprendizagemPlano> ObterObjetivosAprendizagemPorIdPlano(long idPlano);

        IEnumerable<ObjetivoAprendizagemPlano> ObterObjetivosPlanoDisciplina(int ano, int bimestre, long turmaId, long componenteCurricularId, long disciplinaId, bool ehRegencia = false);
    }
}