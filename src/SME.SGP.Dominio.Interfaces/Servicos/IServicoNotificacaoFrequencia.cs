using System;
using System.Threading.Tasks;

namespace SME.SGP.Dominio.Interfaces
{
    public interface IServicoNotificacaoFrequencia
    {
        Task ExecutaNotificacaoRegistroFrequencia();

        void NotificarAlunosFaltosos();

        void NotificarAlunosFaltososBimestre();

        void NotificarCompensacaoAusencia(long compensacaoId);

        void VerificaNotificacaoBimestral();

        void VerificaRegraAlteracaoFrequencia(long registroFrequenciaId, DateTime criadoEm, DateTime alteradoEm, long usuarioAlteracaoId);
    }
}