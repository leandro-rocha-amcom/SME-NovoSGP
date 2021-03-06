import api from '~/servicos/api';

const AbrangenciaServico = {
  buscarDres(url = '') {
    if (url) return api.get(url);
    return api.get(`/v1/abrangencias/false/dres`);
  },
  buscarUes(dreId, url = '') {
    if (url) return api.get(`${url}/${dreId}/ues/atribuicoes`);
    return api.get(`/v1/abrangencias/false/dres/${dreId}/ues`);
  },
  buscarModalidades() {
    return api.get(`v1/abrangencias/modalidades`);
  },
  /**
   * @param {String} ue Ue selecionada
   * @param {String} modalidade Modalidade Selecionada
   * @param {String} periodo Periodo (opcional)
   */
  buscarTurmas(ue, modalidade = 0, periodo = '') {
    let params = { modalidade };
    if (periodo) {
      params = { ...params, periodo };
    }

    return api.get(`v1/abrangencias/false/dres/ues/${ue}/turmas`, {
      params,
    });
  },
};

export default AbrangenciaServico;
