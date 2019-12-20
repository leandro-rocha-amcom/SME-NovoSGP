﻿using Sentry;
using SME.Background.Core;
using SME.SGP.Aplicacao.Integracoes;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Enumerados;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao.Servicos
{
    public class ServicoAbrangencia : IServicoAbrangencia
    {
        private readonly IConsultasSupervisor consultasSupervisor;
        private readonly IRepositorioAbrangencia repositorioAbrangencia;
        private readonly IRepositorioDre repositorioDre;
        private readonly IRepositorioTurma repositorioTurma;
        private readonly IRepositorioUe repositorioUe;
        private readonly IServicoEOL servicoEOL;
        private readonly IUnitOfWork unitOfWork;

        public ServicoAbrangencia(IRepositorioAbrangencia repositorioAbrangencia, IUnitOfWork unitOfWork, IServicoEOL servicoEOL, IConsultasSupervisor consultasSupervisor,
            IRepositorioDre repositorioDre, IRepositorioUe repositorioUe, IRepositorioTurma repositorioTurma)
        {
            this.repositorioAbrangencia = repositorioAbrangencia ?? throw new ArgumentNullException(nameof(repositorioAbrangencia));
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.servicoEOL = servicoEOL ?? throw new ArgumentNullException(nameof(servicoEOL));
            this.consultasSupervisor = consultasSupervisor ?? throw new ArgumentNullException(nameof(consultasSupervisor));
            this.repositorioDre = repositorioDre ?? throw new ArgumentNullException(nameof(repositorioDre));
            this.repositorioUe = repositorioUe ?? throw new ArgumentNullException(nameof(repositorioUe));
            this.repositorioTurma = repositorioTurma ?? throw new ArgumentNullException(nameof(repositorioTurma));
        }

        public void RemoverAbrangencias(long[] ids)
        {
            repositorioAbrangencia.ExcluirAbrangencias(ids);
        }

        public async Task Salvar(string login, Guid perfil, bool ehLogin)
        {
            if (ehLogin)
                await TrataAbrangenciaLogin(login, perfil);
            else await TrataAbrangenciaModificaoPerfil(login, perfil);
        }

        public void SalvarAbrangencias(IEnumerable<Abrangencia> abrangencias, string login)
        {
            repositorioAbrangencia.InserirAbrangencias(abrangencias, login);
        }

        public void SincronizarDesvinculoTurma(string login, Guid perfil)
        {
            var turmasMarcadas = repositorioAbrangencia.ObterTurmasMarcadasParaDesvinculo(login, perfil);

            if (turmasMarcadas != null && turmasMarcadas.Count() > 0)
            {
                var turmasFinalizadasConsulta = servicoEOL.ObterTurmasFinalizadas(null, turmasMarcadas.Select(x => x.CodigoTurma).ToArray());
                var vinculosFinalizadosConsulta = servicoEOL.ObterVinculosFinalizados(login);

                var turmasFinalizadas = turmasFinalizadasConsulta.Result;

                if (turmasFinalizadas.Count() > 0)
                {
                    foreach (var item in turmasFinalizadas)
                        repositorioTurma.FinalizarTurma(item.CodigoTurma, item.DataFim);
                }

                var vinculosFinalizados = vinculosFinalizadosConsulta.Result;

                if (vinculosFinalizados.Count() > 0)
                {
                    foreach (var item in vinculosFinalizados)
                        repositorioAbrangencia.FinalizarVinculos(login, perfil, item.CodigoTurma, item.DataFimVinculo);
                }

                var turmasNaoCobertas = turmasMarcadas.Where(x => !vinculosFinalizados.Any(y => y.CodigoTurma == x.CodigoTurma) && !turmasFinalizadas.Any(y => y.CodigoTurma == x.CodigoTurma));

                // Se por qualquer motivo marcamos indevidademente o fim da abrangencia, desfazemos!
                if (turmasNaoCobertas != null && turmasNaoCobertas.Count() > 0)
                    repositorioAbrangencia.DesfazerMarcacaoAbrangenciasNaoVinculadas(login, perfil, turmasNaoCobertas);
            }
        }

        public void SincronizarTurmasEncerradas()
        {
            var ultimoProcessamento = repositorioAbrangencia.ObterDataUltimoProcessamento() ?? new DateTime(DateTime.Today.Year, 1, 1);

            var turmasEncerradas = servicoEOL.ObterTurmasFinalizadas(ultimoProcessamento, null).Result;

            foreach (var item in turmasEncerradas)
                repositorioTurma.FinalizarTurma(item.CodigoTurma, item.DataFim);

            repositorioAbrangencia.AtualizarUltimoProcessamento(turmasEncerradas.Max(x=> x.DataFim));
        }

        public void SincronizarEstruturaInstitucionalVigenteCompleta()
        {
            var estruturaInstitucionalVigente = servicoEOL.ObterEstruturaInstuticionalVigentePorDre();

            if (estruturaInstitucionalVigente != null && estruturaInstitucionalVigente.Dres != null && estruturaInstitucionalVigente.Dres.Count > 0)
                SincronizarEstruturaInstitucional(estruturaInstitucionalVigente);
            else
            {
                var erro = new NegocioException("Não foi possível obter dados de estrutura institucional do EOL");
                SentrySdk.CaptureException(erro);
                throw erro;
            }
        }

        private async Task BuscaAbrangenciaEPersiste(string login, Guid perfil)
        {
            try
            {
                const string breadcrumb = "SGP API - Tratamento de Abrangência";

                Task<AbrangenciaCompactaVigenteRetornoEOLDTO> consultaEol = null;

                var ehSupervisor = perfil == Perfis.PERFIL_SUPERVISOR;
                var ehProfessorCJ = perfil == Perfis.PERFIL_CJ;

                SentrySdk.AddBreadcrumb($"{breadcrumb} - Chamada BuscaAbrangenciaEPersiste - Login: {login}, perfil {perfil} - EhSupervisor: {ehSupervisor}, EhProfessorCJ: {ehProfessorCJ}", "SGP Api - Negócio");

                if (ehSupervisor)
                    consultaEol = TratarRetornoSupervisor(ObterAbrangenciaEolSupervisor(login));
                else if (ehProfessorCJ)
                    return;
                else
                    consultaEol = servicoEOL.ObterAbrangenciaCompactaVigente(login, perfil);

                // Enquanto o EOl consulta, tentamos ganhar tempo obtendo a consulta sintetica
                var consultaAbrangenciaSintetica = repositorioAbrangencia.ObterAbrangenciaSintetica(login, perfil);

                var abrangenciaEol = await consultaEol;
                var abrangenciaSintetica = await consultaAbrangenciaSintetica;

                if (abrangenciaEol == null)
                    throw new NegocioException("Não foi possível localizar registros de abrangência para este usuário.");

                IEnumerable<Dre> dres = Enumerable.Empty<Dre>();
                IEnumerable<Ue> ues = Enumerable.Empty<Ue>();
                IEnumerable<Turma> turmas = Enumerable.Empty<Turma>();

                // sincronizamos as dres, ues e turmas
                MaterializarEstruturaInstitucional(abrangenciaEol, ref dres, ref ues, ref turmas);

                // sincronizamos a abrangencia do login + perfil
                SincronizarAbrangencia(abrangenciaSintetica, abrangenciaEol.Abrangencia.Abrangencia, ehSupervisor, dres, ues, turmas, login, perfil);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        private IEnumerable<Turma> ImportarTurmasNaoEncontradas(string[] codigosNaoEncontrados)
        {
            IEnumerable<Turma> resultado = Enumerable.Empty<Turma>();

            if (codigosNaoEncontrados != null && codigosNaoEncontrados.Length > 0)
            {
                var turmasEol = servicoEOL.ObterEstruturaInstuticionalVigentePorTurma(codigosTurma: codigosNaoEncontrados);
                if (turmasEol != null)
                    SincronizarEstruturaInstitucional(turmasEol);
            }

            return repositorioTurma.MaterializarCodigosTurma(codigosNaoEncontrados, out codigosNaoEncontrados);
        }

        private void MaterializarEstruturaInstitucional(AbrangenciaCompactaVigenteRetornoEOLDTO abrangenciaEol, ref IEnumerable<Dre> dres, ref IEnumerable<Ue> ues, ref IEnumerable<Turma> turmas)
        {
            string[] codigosNaoEncontrados;

            if (abrangenciaEol.IdDres != null && abrangenciaEol.IdDres.Length > 0)
                dres = repositorioDre.MaterializarCodigosDre(abrangenciaEol.IdDres, out codigosNaoEncontrados);

            if (abrangenciaEol.IdUes != null && abrangenciaEol.IdUes.Length > 0)
                ues = repositorioUe.MaterializarCodigosUe(abrangenciaEol.IdUes, out codigosNaoEncontrados);

            if (abrangenciaEol.IdTurmas != null && abrangenciaEol.IdTurmas.Length > 0)
            {
                turmas = repositorioTurma.MaterializarCodigosTurma(abrangenciaEol.IdTurmas, out codigosNaoEncontrados)
                    .Union(ImportarTurmasNaoEncontradas(codigosNaoEncontrados));
            }
        }

        private Task<AbrangenciaRetornoEolDto> ObterAbrangenciaEolSupervisor(string login)
        {
            Task<AbrangenciaRetornoEolDto> consultaEol;
            var listaEscolasDresSupervior = consultasSupervisor.ObterPorDreESupervisor(login, string.Empty);

            if (!listaEscolasDresSupervior.Any())
                throw new NegocioException($"Não foi possível obter as escolas atribuidas ao supervisor {login}.");

            var escolas = from a in listaEscolasDresSupervior
                          from b in a.Escolas
                          select b.Codigo;

            consultaEol = servicoEOL.ObterAbrangenciaParaSupervisor(escolas.ToArray());
            return consultaEol;
        }

        private void SincronizarAbragenciaPorTurmas(IEnumerable<AbrangenciaSinteticaDto> abrangenciaSintetica, IEnumerable<Turma> turmas, string login, Guid perfil)
        {
            repositorioAbrangencia.RemoverAbrangenciasForaEscopo(login, perfil, TipoAbrangencia.PorTurma);

            var novas = turmas.Where(x => !abrangenciaSintetica.Select(y => y.TurmaId).Contains(x.Id));

            repositorioAbrangencia.InserirAbrangencias(novas.Select(x => new Abrangencia() { Perfil = perfil, TurmaId = x.Id }), login);

            var paraExcluir = abrangenciaSintetica.Where(x => !turmas.Select(y => y.Id).Contains(x.TurmaId)).Select(x => x.Id);

            repositorioAbrangencia.MarcarAbrangenciasNaoVinculadas(paraExcluir);

            if (paraExcluir != null && paraExcluir.Count() > 0)
                Cliente.Executar<IServicoAbrangencia>(x => x.SincronizarDesvinculoTurma(login, perfil));
        }

        private void SincronizarAbrangencia(IEnumerable<AbrangenciaSinteticaDto> abrangenciaSintetica, Infra.Enumerados.Abrangencia abrangencia, bool ehSupervisor, IEnumerable<Dre> dres, IEnumerable<Ue> ues, IEnumerable<Turma> turmas, string login, Guid perfil)
        {
            if (ehSupervisor)
                SincronizarAbrangenciaPorUes(abrangenciaSintetica, ues, login, perfil);
            else
            {
                switch (abrangencia)
                {
                    case Infra.Enumerados.Abrangencia.Dre:
                    case Infra.Enumerados.Abrangencia.SME:
                        SincronizarAbrangenciPorDres(abrangenciaSintetica, dres, login, perfil);
                        break;

                    case Infra.Enumerados.Abrangencia.DreEscolasAtribuidas:
                    case Infra.Enumerados.Abrangencia.UE:
                        SincronizarAbrangenciaPorUes(abrangenciaSintetica, ues, login, perfil);
                        break;

                    case Infra.Enumerados.Abrangencia.UeTurmasDisciplinas:
                    case Infra.Enumerados.Abrangencia.Professor:
                        SincronizarAbragenciaPorTurmas(abrangenciaSintetica, turmas, login, perfil);
                        break;
                }
            }
        }

        private void SincronizarAbrangenciaPorUes(IEnumerable<AbrangenciaSinteticaDto> abrangenciaSintetica, IEnumerable<Ue> ues, string login, Guid perfil)
        {
            repositorioAbrangencia.RemoverAbrangenciasForaEscopo(login, perfil, TipoAbrangencia.PorUe);

            var novas = ues.Where(x => !abrangenciaSintetica.Select(y => y.UeId).Contains(x.Id));

            repositorioAbrangencia.InserirAbrangencias(novas.Select(x => new Abrangencia() { Perfil = perfil, UeId = x.Id }), login);

            var paraExcluir = abrangenciaSintetica.Where(x => !ues.Select(y => y.Id).Contains(x.UeId)).Select(x => x.Id);

            repositorioAbrangencia.ExcluirAbrangencias(paraExcluir);
        }

        private void SincronizarAbrangenciPorDres(IEnumerable<AbrangenciaSinteticaDto> abrangenciaSintetica, IEnumerable<Dre> dres, string login, Guid perfil)
        {
            repositorioAbrangencia.RemoverAbrangenciasForaEscopo(login, perfil, TipoAbrangencia.PorDre);

            var novas = dres.Where(x => !abrangenciaSintetica.Select(y => y.DreId).Contains(x.Id));

            repositorioAbrangencia.InserirAbrangencias(novas.Select(x => new Abrangencia() { Perfil = perfil, DreId = x.Id }), login);

            var paraExcluir = abrangenciaSintetica.Where(x => !dres.Select(y => y.Id).Contains(x.DreId)).Select(x => x.Id);

            repositorioAbrangencia.ExcluirAbrangencias(paraExcluir);
        }

        private void SincronizarEstruturaInstitucional(EstruturaInstitucionalRetornoEolDTO estrutura)
        {
            IEnumerable<Dre> dres = Enumerable.Empty<Dre>();
            IEnumerable<Ue> ues = Enumerable.Empty<Ue>();
            IEnumerable<Turma> turmas = Enumerable.Empty<Turma>();

            dres = estrutura.Dres.Select(x => new Dre() { Abreviacao = x.Abreviacao, CodigoDre = x.Codigo, Nome = x.Nome });
            ues = estrutura.Dres.SelectMany(x => x.Ues.Select(y => new Ue { CodigoUe = y.Codigo, TipoEscola = y.CodTipoEscola, Nome = y.Nome, Dre = new Dre() { CodigoDre = x.Codigo } }));
            turmas = estrutura.Dres.SelectMany(x => x.Ues.SelectMany(y => y.Turmas.Select(z =>
            new Turma
            {
                Ano = z.Ano,
                AnoLetivo = z.AnoLetivo,
                CodigoTurma = z.Codigo,
                ModalidadeCodigo = (Modalidade)Convert.ToInt32(z.CodigoModalidade),
                QuantidadeDuracaoAula = z.DuracaoTurno,
                Nome = z.NomeTurma,
                Semestre = z.Semestre,
                TipoTurno = z.TipoTurno,
                Ue = new Ue() { CodigoUe = y.Codigo }
            })));

            dres = repositorioDre.Sincronizar(dres);
            ues = repositorioUe.Sincronizar(ues, dres);
            repositorioTurma.Sincronizar(turmas, ues);
        }

        private async Task TrataAbrangenciaLogin(string login, Guid perfil)
        {
            unitOfWork.IniciarTransacao();
            await BuscaAbrangenciaEPersiste(login, perfil);
            unitOfWork.PersistirTransacao();
        }

        private async Task TrataAbrangenciaModificaoPerfil(string login, Guid perfil)
        {
            if (!(await repositorioAbrangencia.JaExisteAbrangencia(login, perfil)))
            {
                unitOfWork.IniciarTransacao();
                await BuscaAbrangenciaEPersiste(login, perfil);
                unitOfWork.PersistirTransacao();
            }
        }

        private Task<AbrangenciaCompactaVigenteRetornoEOLDTO> TratarRetornoSupervisor(Task<AbrangenciaRetornoEolDto> consultaEol)
        {
            if (consultaEol != null)
            {
                return Task.Factory.StartNew(() =>
                {
                    var resultado = consultaEol.Result;
                    return new AbrangenciaCompactaVigenteRetornoEOLDTO()
                    {
                        Abrangencia = resultado.Abrangencia,
                        IdUes = resultado.Dres.SelectMany(x => x.Ues.Select(y => y.Codigo)).ToArray()
                    };
                });
            }
            return null;
        }
    }
}