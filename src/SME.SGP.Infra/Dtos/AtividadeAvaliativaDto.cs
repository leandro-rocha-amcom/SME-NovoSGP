﻿using SME.SGP.Dominio;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SME.SGP.Infra
{
    public class AtividadeAvaliativaDto
    {
        [EnumeradoRequirido(ErrorMessage = "A categoria é obrigatória.")]
        public CategoriaAtividadeAvaliativa CategoriaId { get; set; }

        [DataRequerida(ErrorMessage = "É necessario informar a data da avaliação")]
        public DateTime DataAvaliacao { get; set; }

        public string Descricao { get; set; }

        [Required(ErrorMessage = "A Disciplina da atividade avaliativa deve ser informada.")]
        public int DisciplinaId { get; set; }

        [Required(ErrorMessage = "A Dre da atividade avaliativa deve ser informado.")]
        [MaxLength(15, ErrorMessage = "A Dre deve conter no máximo 15 caracteres.")]
        public string DreId { get; set; }

        [IgnoreDataMember]
        public bool EhRegencia { get; set; }

        [Required(ErrorMessage = "O nome da atividade avaliativa deve ser informado.")]
        [MaxLength(100, ErrorMessage = "O nome deve conter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        public int TipoAvaliacaoId { get; set; }

        [Required(ErrorMessage = "A turma deve ser informada")]
        public string TurmaId { get; set; }

        [Required(ErrorMessage = "A UE da atividade avaliativa deve ser informado.")]
        [MaxLength(15, ErrorMessage = "A UE deve conter no máximo 15 caracteres.")]
        public string UeId { get; set; }
    }
}