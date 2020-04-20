﻿using Dapper;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioTipoEscola : RepositorioBase<TipoEscolaEol>, IRepositorioTipoEscola
    {
        private const string QuerySincronizacao = @"SELECT id, cod_tipo_escola_eol,  descricao, data_atualizacao FROM public.tipo_escola where cod_tipo_escola_eol in (#ids);";

        public RepositorioTipoEscola(ISgpContext database) : base(database)
        {
        }

        public void Sincronizar(IEnumerable<TipoEscolaEol> tiposEscola)
        {
            var armazenados = database.Conexao.Query<TipoEscolaEol>(QuerySincronizacao.Replace("#ids", string.Join(",", tiposEscola.Select(x => $"'{x.CodEol}'"))));
            var novos = tiposEscola.Where(x => !armazenados.Select(y => y.CodEol).Contains(x.CodEol));
            foreach (var item in novos)
            {
                item.DtAtualizacao = DateTime.Today;
                Salvar(item);
            }

            var modificados = from c in tiposEscola
                              join l in armazenados on c.CodEol equals l.CodEol
                              where l.DtAtualizacao != DateTime.Today &&
                                    (c.Descricao != l.Descricao)
                              select new TipoEscolaEol()
                              {
                                  Id = l.Id,
                                  Descricao = c.Descricao,
                                  DtAtualizacao = DateTime.Today
                              };

            foreach (var item in modificados)
                Salvar(item);
        }
    }
}