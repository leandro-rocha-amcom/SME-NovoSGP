﻿using System;

namespace SME.SGP.Dominio
{
    public class Aula : EntidadeBase
    {
        public DateTime DataAula { get; set; }
        public string DisciplinaId { get; set; }
        public bool Excluido { get; set; }
        public bool Migrado { get; set; }
        public long ProfessorId { get; set; }
        public int Quantidade { get; set; }
        public RecorrenciaAula RecorrenciaAula { get; set; }
        public TipoAula TipoAula { get; set; }
        public long TipoCalendarioId { get; set; }
        public string TurmaId { get; set; }
        public string UeId { get; set; }

        //public bool EstaNoPeriodoLetivo(IEnumerable<PeriodoEscolar> periodosEscolares)
        //{
        //    if (periodosEscolares == null || !periodosEscolares.Any())
        //        throw new NegocioException("Períodos escolares não encontrados.");

        //    return periodosEscolares.Any(c => c.PeriodoInicio >= DataAula && c.PeriodoFim <= DataAula);
        //}
    }
}