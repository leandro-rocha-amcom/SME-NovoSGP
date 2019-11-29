﻿using SME.SGP.Dominio;
using SME.SGP.Infra;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public interface IComandosAula
    {
        Task<string> Alterar(AulaDto dto, long id);

        void Excluir(long id, RecorrenciaAula recorrencia);

        Task<string> Inserir(AulaDto dto);
    }
}