﻿using SME.SGP.Infra;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public interface IComandosPlanoAula
    {
        Task Salvar(PlanoAulaDto planoAulaDto, bool controlarTransacao = true);

        Task Migrar(MigrarPlanoAulaDto migrarPlanoAulaDto);

        Task ExcluirPlanoDaAula(long aulaId);
    }
}
