using SME.SGP.Dto;
using System.Collections.Generic;

namespace SME.SGP.Aplicacao
{
    public interface IComandosNotificacao
    {
        void AtualizarParaLida(IEnumerable<long> notificacaoId);

        void Salvar(NotificacaoDto notificacaoDto);
    }
}