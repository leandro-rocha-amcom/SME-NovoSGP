﻿using System.ComponentModel.DataAnnotations;

namespace SME.SGP.Dto
{
    public class BuscaSupervisorPorNomeDto
    {
        [MinLength(2, ErrorMessage = "Nome deve conter no mínimo 2 caracteres")]
        public string Nome { get; set; }
    }
}