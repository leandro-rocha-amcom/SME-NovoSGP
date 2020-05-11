using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public class ConsultasConselhoClasseNota : IConsultasConselhoClasseNota
    {
        private readonly IRepositorioConselhoClasseNota repositorioConselhoClasseNota;
        public ConsultasConselhoClasseNota(IRepositorioConselhoClasseNota repositorioConselhoClasseNota)
        {
            this.repositorioConselhoClasseNota = repositorioConselhoClasseNota ?? throw new ArgumentNullException(nameof(repositorioConselhoClasseNota));
        }

        public ConselhoClasseNota ObterPorId(long id)
        {
            return repositorioConselhoClasseNota.ObterPorId(id);
        }
        public async Task<IEnumerable<NotaConceitoBimestreComponenteDto>> ObterNotasAlunoAsync(long conselhoClasseId, string alunoCodigo)
            => await repositorioConselhoClasseNota.ObterNotasAlunoAsync(conselhoClasseId, alunoCodigo);
    }
}