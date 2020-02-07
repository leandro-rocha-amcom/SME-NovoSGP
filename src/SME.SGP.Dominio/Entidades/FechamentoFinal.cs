﻿namespace SME.SGP.Dominio
{
    public class FechamentoFinal : EntidadeBase
    {
        public string AlunoCodigo { get; set; }
        public int AusenciasCompensadas { get; set; }
        public Conceito Conceito { get; set; }
        public long ConceitoId { get; set; }
        public string DisciplinaCodigo { get; set; }
        public double Nota { get; set; }
        public Turma Turma { get; set; }
        public long TurmaId { get; set; }
    }
}