﻿using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;

namespace SME.SGP.Dominio.Servicos
{
    public class ServicoAluno : IServicoAluno
    {
        public MarcadorFrequenciaDto ObterMarcadorAluno(AlunoPorTurmaResposta aluno, PeriodoEscolarDto bimestre)
        {
            MarcadorFrequenciaDto marcador = null;

            string dataSituacao = $"{aluno.DataSituacao.Day}/{aluno.DataSituacao.Month}/{aluno.DataSituacao.Year}";
            switch (aluno.CodigoSituacaoMatricula)
            {
                case SituacaoMatriculaAluno.Ativo:
                    // Macador "Novo" durante 15 dias se iniciou depois do inicio do bimestre
                    if ((aluno.DataSituacao > bimestre.PeriodoInicio) && (aluno.DataSituacao.AddDays(15) <= DateTime.Now.Date))
                        marcador = new MarcadorFrequenciaDto()
                        {
                            Tipo = TipoMarcadorFrequencia.Novo,
                            Descricao = $"Estudante Novo: Data da matrícula {dataSituacao}"
                        };
                    break;

                case SituacaoMatriculaAluno.Transferido:
                    var detalheEscola = aluno.Transferencia_Interna ?
                                        $"para escola {aluno.EscolaTransferencia} e turma {aluno.TurmaTransferencia}" :
                                        "para outras redes";

                    marcador = new MarcadorFrequenciaDto()
                    {
                        Tipo = TipoMarcadorFrequencia.Transferido,
                        Descricao = $"Estudante transferido: {detalheEscola} em {dataSituacao}"
                    };

                    break;

                case SituacaoMatriculaAluno.RemanejadoSaida:
                    marcador = new MarcadorFrequenciaDto()
                    {
                        Tipo = TipoMarcadorFrequencia.Remanejado,
                        Descricao = $"Estudante remanejado: turma {aluno.TurmaRemanejamento} em {dataSituacao}"
                    };

                    break;

                case SituacaoMatriculaAluno.Desistente:
                case SituacaoMatriculaAluno.VinculoIndevido:
                case SituacaoMatriculaAluno.Falecido:
                case SituacaoMatriculaAluno.NaoCompareceu:
                case SituacaoMatriculaAluno.Deslocamento:
                case SituacaoMatriculaAluno.Cessado:
                case SituacaoMatriculaAluno.ReclassificadoSaida:
                    marcador = new MarcadorFrequenciaDto()
                    {
                        Tipo = TipoMarcadorFrequencia.Inativo,
                        Descricao = $"Aluno inativo em {dataSituacao}"
                    };

                    break;

                default:
                    break;
            }

            return marcador;
        }
    }
}