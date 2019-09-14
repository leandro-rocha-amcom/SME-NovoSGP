﻿using SME.SGP.Dto;
using System.Collections.Generic;

namespace SME.SGP.Aplicacao
{
    public interface IComandosNotificacao
    {
        List<AlteracaoStatusNotificacaoDto> MarcarComoLida(IList<long> notificacoesId);

        void Salvar(NotificacaoDto notificacaoDto);
    }
}