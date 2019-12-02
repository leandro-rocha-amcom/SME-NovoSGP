﻿using Dapper;
using SME.SGP.Dados.Contexto;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioAtividadeAvaliativa : RepositorioBase<AtividadeAvaliativa>, IRepositorioAtividadeAvaliativa
    {
        private readonly string fromCompleto = "from atividade_avaliativa a inner join tipo_avaliacao ta on a.tipo_avaliacao_id = ta.id";

        public RepositorioAtividadeAvaliativa(ISgpContext conexao) : base(conexao)
        {
        }

        public async Task<PaginacaoResultadoDto<AtividadeAvaliativa>> Listar(DateTime? dataAvaliacao, string dreId, string ueId, string nomeAvaliacao, int? tipoAvaliacaoId, string turmaId, Paginacao paginacao)
        {
            if (!string.IsNullOrEmpty(nomeAvaliacao))
                nomeAvaliacao = $"%{nomeAvaliacao.ToLowerInvariant()}%";
            StringBuilder query = new StringBuilder();
            if (paginacao == null)
                paginacao = new Paginacao(1, 10);

            MontaQueryCabecalho(query);
            query.AppendLine(fromCompleto);
            MontaWhere(query, dataAvaliacao, dreId, ueId, nomeAvaliacao, tipoAvaliacaoId, turmaId, null);

            if (paginacao.QuantidadeRegistros != 0)
                query.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", paginacao.QuantidadeRegistrosIgnorados, paginacao.QuantidadeRegistros);

            var retornoPaginado = new PaginacaoResultadoDto<AtividadeAvaliativa>()
            {
                Items = await database.Conexao.QueryAsync<AtividadeAvaliativa, TipoAvaliacao, AtividadeAvaliativa>(query.ToString(), (atividadeAvaliativa, tipoAvaliacao) =>
                  {
                      atividadeAvaliativa.AdicionarTipoAvaliacao(tipoAvaliacao);
                      return atividadeAvaliativa;
                  }, new
                  {
                      dataAvaliacao,
                      dreId,
                      ueId,
                      nomeAvaliacao,
                      tipoAvaliacaoId,
                      turmaId
                  },
            splitOn: "AtividadeAvaliativaId,TipoAvaliacaoId")
            };

            var queryCountCabecalho = "select count(distinct a.id)";
            var queryCount = new StringBuilder(queryCountCabecalho);
            queryCount.AppendLine(fromCompleto);
            MontaWhere(queryCount, dataAvaliacao, dreId, ueId, nomeAvaliacao, tipoAvaliacaoId, turmaId, null);

            retornoPaginado.TotalRegistros = (await database.Conexao.QueryAsync<int>(queryCount.ToString(), new
            {
                dataAvaliacao,
                dreId,
                ueId,
                nomeAvaliacao,
                tipoAvaliacaoId,
                turmaId
            })).Sum();

            retornoPaginado.TotalPaginas = (int)Math.Ceiling((double)retornoPaginado.TotalRegistros / paginacao.QuantidadeRegistros);

            return retornoPaginado;
        }

        public async Task<bool> VerificarSeJaExisteAvaliacao(DateTime dataAvaliacao, string dreId, string ueId, string turmaId, string professorRf)
        {
            StringBuilder query = new StringBuilder();
            MontaQueryCabecalho(query);
            query.AppendLine(fromCompleto);
            MontaWhere(query, dataAvaliacao, dreId, ueId, null, null, turmaId, professorRf);

            var resultado = (await database.Conexao.QueryAsync<AtividadeAvaliativa>(query.ToString(), new
            {
                dataAvaliacao,
                dreId,
                ueId,
                turmaId,
                professorRf
            }));

            return resultado.Any();
        }

        public async Task<bool> VerificarSeJaExisteAvaliacaoComMesmoNome(string nomeAvaliacao, string dreId, string ueId, string turmaId, string professorRf, DateTime periodoInicio, DateTime periodoFim)
        {
            nomeAvaliacao = nomeAvaliacao.ToLowerInvariant();
            StringBuilder query = new StringBuilder();
            MontaQueryCabecalho(query);
            query.AppendLine(fromCompleto);
            MontaWhere(query, null, dreId, ueId, nomeAvaliacao, null, turmaId, professorRf, periodoInicio, periodoFim, true);

            var resultado = (await database.Conexao.QueryAsync<AtividadeAvaliativa>(query.ToString(), new
            {
                dreId,
                ueId,
                nomeAvaliacao,
                turmaId,
                professorRf,
                periodoInicio,
                periodoFim
            }));

            return resultado.Any();
        }

        public async Task<bool> VerificarSeJaExisteAvaliacaoRegencia(DateTime dataAvaliacao, string dreId, string ueId, string turmaId, string disciplinaId, string professorRf)
        {
            StringBuilder query = new StringBuilder();
            MontaQueryCabecalho(query);
            query.AppendLine(fromCompleto);
            MontaWhere(query, dataAvaliacao, dreId, ueId, null, null, turmaId, professorRf, null, null, false, disciplinaId);

            var resultado = (await database.Conexao.QueryAsync<AtividadeAvaliativa>(query.ToString(), new
            {
                dataAvaliacao,
                dreId,
                ueId,
                turmaId,
                professorRf,
                disciplinaId
            }));

            return resultado.Any();
        }

        private static void MontaQueryCabecalho(StringBuilder query)
        {
            query.AppendLine("select");
            query.AppendLine("a.id as AtividadeAvaliativaId,");
            query.AppendLine("a.id,");
            query.AppendLine("a.dre_id,");
            query.AppendLine("a.ue_id,");
            query.AppendLine("a.professor_rf,");
            query.AppendLine("a.turma_id,");
            query.AppendLine("a.categoria_id,");
            query.AppendLine("a.tipo_avaliacao_id,");
            query.AppendLine("a.nome_avaliacao,");
            query.AppendLine("a.descricao_avaliacao,");
            query.AppendLine("a.data_avaliacao,");
            query.AppendLine("a.criado_em,");
            query.AppendLine("a.criado_por,");
            query.AppendLine("a.alterado_em,");
            query.AppendLine("a.alterado_por,");
            query.AppendLine("a.criado_rf,");
            query.AppendLine("a.alterado_rf,");
            query.AppendLine("a.excluido,");
            query.AppendLine("ta.id as TipoAvaliacaoId,");
            query.AppendLine("ta.id,");
            query.AppendLine("ta.nome,");
            query.AppendLine("ta.descricao,");
            query.AppendLine("ta.situacao");
        }

        private void MontaWhere(StringBuilder query,
            DateTime? dataAvaliacao,
            string dreId,
            string ueId,
            string nomeAvaliacao,
            int? tipoAvaliacaoId,
            string turmaId,
            string professorRf,
            DateTime? perioInicio = null,
            DateTime? periodoFim = null,
            bool nomeExato = false,
            string disciplinaId = null,
            bool ehRegencia = false)
        {
            query.AppendLine("where");
            query.AppendLine("a.excluido = false");
            query.AppendLine("and ta.situacao = true");
            if (dataAvaliacao.HasValue)
                query.AppendLine("and date(a.data_avaliacao) = @dataAvaliacao");
            if (!string.IsNullOrEmpty(dreId))
                query.AppendLine("and a.dre_id = @dreId");
            if (!string.IsNullOrEmpty(ueId))
                query.AppendLine("and a.ue_id = @ueId");
            if (!string.IsNullOrEmpty(nomeAvaliacao))
                if (nomeExato)
                    query.AppendLine("and  lower(f_unaccent(a.nome_avaliacao)) = f_unaccent(@nomeAvaliacao)");
                else
                    query.AppendLine("and  lower(f_unaccent(a.nome_avaliacao)) LIKE f_unaccent(@nomeAvaliacao)");
            if (!string.IsNullOrEmpty(turmaId))
                query.AppendLine("and a.turma_id = @turmaId");
            if (tipoAvaliacaoId.HasValue)
                query.AppendLine("and ta.id = @tipoAvaliacaoId");
            if (!string.IsNullOrEmpty(professorRf))
                query.AppendLine("and a.professor_rf = @professorRf");
            if (perioInicio.HasValue)
                query.AppendLine("and date(a.data_avaliacao) >= @periodoInicio");
            if (periodoFim.HasValue)
                query.AppendLine("and date(a.data_avaliacao) <= @periodoFim");
            if (!string.IsNullOrEmpty(disciplinaId))
                query.AppendLine("and a.disciplina_id = @disciplinaId");
            if (ehRegencia)
                query.AppendLine("and a.eh_regencia = true");
        }
    }
}