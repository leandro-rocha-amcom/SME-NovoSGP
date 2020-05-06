﻿using SME.SGP.Aplicacao;
using SME.SGP.Aplicacao.Integracoes;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SGP.Dominio.Servicos
{
    public class ServicoCalculoParecerConclusivo : IServicoCalculoParecerConclusivo
    {
        protected IServicoCalculoParecerConclusivo quandoVerdadeiro;
        protected IServicoCalculoParecerConclusivo quandoFalso;
        protected IEnumerable<ConselhoClasseParecerConclusivo> pareceresDoServico;

        private readonly IRepositorioParametrosSistema repositorioParametrosSistema;
        private readonly IRepositorioFechamentoNota repositorioFechamentoNota;
        private readonly IRepositorioConceito repositorioConceito;
        private readonly IRepositorioConselhoClasseNota repositorioConselhoClasseNota;
        private readonly IConsultasFrequencia consultasFrequencia;
        private readonly IServicoEOL servicoEOL;

        public ServicoCalculoParecerConclusivo(IRepositorioParametrosSistema repositorioParametrosSistema,
                                               IRepositorioFechamentoNota repositorioFechamentoNota,
                                               IRepositorioConceito repositorioConceito,
                                               IRepositorioConselhoClasseNota repositorioConselhoClasseNota,
                                               IConsultasFrequencia consultasFrequencia,
                                               IServicoEOL servicoEOL)
        {
            this.repositorioParametrosSistema = repositorioParametrosSistema ?? throw new ArgumentNullException(nameof(repositorioParametrosSistema));
            this.repositorioFechamentoNota = repositorioFechamentoNota ?? throw new ArgumentNullException(nameof(repositorioFechamentoNota));
            this.repositorioConceito = repositorioConceito ?? throw new ArgumentNullException(nameof(repositorioConceito));
            this.repositorioConselhoClasseNota = repositorioConselhoClasseNota ?? throw new ArgumentNullException(nameof(repositorioConselhoClasseNota));
            this.consultasFrequencia = consultasFrequencia ?? throw new ArgumentNullException(nameof(consultasFrequencia));
            this.servicoEOL = servicoEOL ?? throw new ArgumentNullException(nameof(servicoEOL));
        }

        public bool Filtrar(IEnumerable<ConselhoClasseParecerConclusivo> pareceresDoServico, string nomeClasseCalculo)
        {
            this.pareceresDoServico = pareceresDoServico;

            // Verifica se retornou 1 verdadeiro e 1 falso
            if (pareceresDoServico == null || !pareceresDoServico.Any())
                return false;

            if (!pareceresDoServico.Where(c => c.Aprovado).Any())
                throw new NegocioException($"Não localizado parecer conclusivo aprovado para o calculo por {nomeClasseCalculo}");
            if (pareceresDoServico.Where(c => c.Aprovado).Count() > 1)
                throw new NegocioException($"Encontrado mais de 1 parecer conclusivo aprovado para o calculo por {nomeClasseCalculo}");

            if (!pareceresDoServico.Where(c => !c.Aprovado).Any())
                throw new NegocioException($"Não localizado parecer conclusivo reprovado para o calculo por {nomeClasseCalculo}");
            if (pareceresDoServico.Where(c => !c.Aprovado).Count() > 1)
                throw new NegocioException($"Encontrado mais de 1 parecer conclusivo reprovado para o calculo por {nomeClasseCalculo}");

            return true;
        }

        private ConselhoClasseParecerConclusivo ObterParecerValidacao(bool retornoValidacao)
            => pareceresDoServico.FirstOrDefault(c => c.Aprovado == retornoValidacao);

        public async Task<ConselhoClasseParecerConclusivo> Calcular(string alunoCodigo, string turmaCodigo, IEnumerable<ConselhoClasseParecerConclusivo> pareceresDaTurma)
        {
            Filtrar(pareceresDaTurma.Where(c => c.Frequencia), "Frequência");
            if (!await ValidarParecerPorFrequencia(alunoCodigo, turmaCodigo, pareceresDaTurma))
                return ObterParecerValidacao(false);

            var parecerFrequencia = ObterParecerValidacao(true);
            if (!Filtrar(pareceresDaTurma.Where(c => c.Nota), "Nota"))
                return parecerFrequencia;

            if (await ValidarParecerPorNota(alunoCodigo, turmaCodigo, pareceresDaTurma))
                return ObterParecerValidacao(true);
            var parecerNota = ObterParecerValidacao(false);

            if (!Filtrar(pareceresDaTurma.Where(c => c.Conselho), "Conselho"))
                return parecerNota;

            return ObterParecerValidacao(await ValidarParecerPorConselho(alunoCodigo, turmaCodigo, pareceresDaTurma));
        }

        #region Frequência
        private async Task<bool> ValidarParecerPorFrequencia(string alunoCodigo, string turmaCodigo, IEnumerable<ConselhoClasseParecerConclusivo> pareceresDaTurma)
        {
            if (!await ValidarFrequenciaGeralAluno(alunoCodigo, turmaCodigo))
                return false;

            return await ValidarFrequenciaBaseNacionalAluno(alunoCodigo, turmaCodigo);
        }
        private async Task<bool> ValidarFrequenciaBaseNacionalAluno(string alunoCodigo, string turmaCodigo)
        {
            var parametroFrequenciaBaseNacional = double.Parse(repositorioParametrosSistema.ObterValorPorTipoEAno(TipoParametroSistema.PercentualFrequenciaCriticoBaseNacional));
            var componentesCurriculares = await servicoEOL.ObterDisciplinasPorCodigoTurma(turmaCodigo);
            // Filtra componentes da Base Nacional
            var componentesCurricularesBaseNacional = componentesCurriculares.Where(c => c.BaseNacional);
            foreach (var componenteCurricular in componentesCurricularesBaseNacional)
            {
                var frequenciaGeralComponente = await consultasFrequencia.ObterFrequenciaGeralAluno(alunoCodigo, turmaCodigo, componenteCurricular.CodigoComponenteCurricular.ToString());
                if (frequenciaGeralComponente < parametroFrequenciaBaseNacional)
                    return false;
            }

            return true;
        }

        private async Task<bool> ValidarFrequenciaGeralAluno(string alunoCodigo, string turmaCodigo)
        {
            var frequenciaAluno = await consultasFrequencia.ObterFrequenciaGeralAluno(alunoCodigo, turmaCodigo);

            var parametroFrequenciaGeral = double.Parse(repositorioParametrosSistema.ObterValorPorTipoEAno(TipoParametroSistema.PercentualFrequenciaCritico));
            return !(frequenciaAluno < parametroFrequenciaGeral);
        }
        #endregion

        #region Nota
        private async Task<bool> ValidarParecerPorNota(string alunoCodigo, string turmaCodigo, IEnumerable<ConselhoClasseParecerConclusivo> pareceresDaTurma)
        {
            var notasFechamentoAluno = await repositorioFechamentoNota.ObterNotasFinaisAlunoAsync(turmaCodigo, alunoCodigo);
            if (notasFechamentoAluno == null || !notasFechamentoAluno.Any())
                return true;

            var tipoNota = notasFechamentoAluno.First().ConceitoId.HasValue ? TipoNota.Conceito : TipoNota.Nota;
            return tipoNota == TipoNota.Nota ?
                ValidarParecerPorNota(notasFechamentoAluno) :
                ValidarParecerPorConceito(notasFechamentoAluno);
        }

        private bool ValidarParecerPorNota(IEnumerable<NotaConceitoBimestreComponenteDto> notasFechamentoAluno)
        {
            var notaMedia = double.Parse(repositorioParametrosSistema.ObterValorPorTipoEAno(TipoParametroSistema.MediaBimestre));
            foreach (var notaFechamentoAluno in notasFechamentoAluno)
                if (notaFechamentoAluno.Nota < notaMedia)
                    return false;

            return true;
        }

        private bool ValidarParecerPorConceito(IEnumerable<NotaConceitoBimestreComponenteDto> conceitosFechamentoAluno)
        {
            var conceitosVigentes = repositorioConceito.ObterPorData(DateTime.Today);
            foreach (var conceitoFechamentoAluno in conceitosFechamentoAluno)
            {
                var conceitoAluno = conceitosVigentes.FirstOrDefault(c => c.Id == conceitoFechamentoAluno.ConceitoId);
                if (!conceitoAluno.Aprovado)
                    return false;
            }

            return true;
        }
        #endregion

        #region Conselho
        private async Task<bool> ValidarParecerPorConselho(string alunoCodigo, string turmaCodigo, IEnumerable<ConselhoClasseParecerConclusivo> pareceresDaTurma)
        {
            var notasConselhoClasse = await repositorioConselhoClasseNota.ObterNotasAlunoAsync(alunoCodigo, turmaCodigo, null);
            if (notasConselhoClasse == null || !notasConselhoClasse.Any())
                return true;

            var tipoNota = notasConselhoClasse.First().ConceitoId.HasValue ? TipoNota.Conceito : TipoNota.Nota;
            return tipoNota == TipoNota.Nota ?
                ValidarParecerConselhoPorNota(notasConselhoClasse) :
                ValidarParecerConselhoPorConceito(notasConselhoClasse);
        }

        private bool ValidarParecerConselhoPorNota(IEnumerable<NotaConceitoBimestreComponenteDto> notasConselhoClasse)
        {
            var notaMedia = double.Parse(repositorioParametrosSistema.ObterValorPorTipoEAno(TipoParametroSistema.MediaBimestre));
            foreach (var notaConcelhoClasse in notasConselhoClasse)
                if (notaConcelhoClasse.Nota < notaMedia)
                    return false;

            return true;
        }

        private bool ValidarParecerConselhoPorConceito(IEnumerable<NotaConceitoBimestreComponenteDto> notasConselhoClasse)
        {
            var conceitosVigentes = repositorioConceito.ObterPorData(DateTime.Today);
            foreach (var conceitoConselhoClasseAluno in notasConselhoClasse)
            {
                var conceitoAluno = conceitosVigentes.FirstOrDefault(c => c.Id == conceitoConselhoClasseAluno.ConceitoId);
                if (!conceitoAluno.Aprovado)
                    return false;
            }

            return true;
        }
        #endregion
    }
}
