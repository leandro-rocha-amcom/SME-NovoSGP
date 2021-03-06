﻿using Dapper;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioPeriodoEscolar : RepositorioBase<PeriodoEscolar>, IRepositorioPeriodoEscolar
    {
        public RepositorioPeriodoEscolar(ISgpContext conexao) : base(conexao)
        { }

        public IEnumerable<PeriodoEscolar> ObterPorTipoCalendario(long tipoCalendarioId)
        {
            string query = "select * from periodo_escolar where tipo_calendario_id = @tipoCalendario";

            return database.Conexao.Query<PeriodoEscolar>(query, new { tipoCalendario = tipoCalendarioId }).ToList();
        }

        public PeriodoEscolar ObterPorTipoCalendarioData(long tipoCalendarioId, DateTime data)
        {
            StringBuilder query = new StringBuilder();
            MontaQuery(query);
            query.AppendLine("where tipo_calendario_id = @tipoCalendarioId");
            query.AppendLine("and periodo_inicio::date <= date(@dataPeriodo)");
            query.AppendLine("and periodo_fim::date >= date(@dataPeriodo)");

            return database.Conexao.QueryFirstOrDefault<PeriodoEscolar>(query.ToString(), new { tipoCalendarioId, dataPeriodo = data.Date });
        }

        public PeriodoEscolar ObterPorTipoCalendarioData(long tipoCalendarioId, DateTime dataInicio, DateTime dataFim)
        {
            StringBuilder query = new StringBuilder();
            MontaQuery(query);
            query.AppendLine("where tipo_calendario_id = @tipoCalendarioId");
            query.AppendLine("and periodo_inicio <= @dataInicio");
            query.AppendLine("and periodo_fim >= @dataFim");

            return database.Conexao.QueryFirstOrDefault<PeriodoEscolar>(query.ToString(), new { tipoCalendarioId, dataInicio, dataFim });
        }

        private static void MontaQuery(StringBuilder query)
        {
            query.AppendLine("select ");
            query.AppendLine("id,");
            query.AppendLine("tipo_calendario_id,");
            query.AppendLine("bimestre,");
            query.AppendLine("periodo_inicio,");
            query.AppendLine("periodo_fim,");
            query.AppendLine("alterado_por,");
            query.AppendLine("alterado_rf,");
            query.AppendLine("alterado_em,");
            query.AppendLine("criado_por,");
            query.AppendLine("criado_rf,");
            query.AppendLine("criado_em");
            query.AppendLine("from periodo_escolar");
        }
    }
}