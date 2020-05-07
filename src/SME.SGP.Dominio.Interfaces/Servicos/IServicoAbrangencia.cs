﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Dominio.Interfaces
{
    public interface IServicoAbrangencia
    {
        void RemoverAbrangencias(long[] ids);

        Task Salvar(string login, Guid perfil, bool ehLogin);

        void SalvarAbrangencias(IEnumerable<Abrangencia> abrangencias, string login);

        void SincronizarEstruturaInstitucionalVigenteCompleta();
    }
}