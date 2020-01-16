import React, { useState, useEffect } from 'react';
import { Form, Formik } from 'formik';
import Cabecalho from '~/componentes-sgp/cabecalho';
import Button from '~/componentes/button';
import Card from '~/componentes/card';
import { Colors } from '~/componentes/colors';
import SelectComponent from '~/componentes/select';
import {
  BoxTextoBimetre,
  CaixaBimestre,
} from './periodo-fechamento-abertura.css';
import api from '~/servicos/api';
import periodo from '~/dtos/periodo';
import { CampoData, Loader } from '~/componentes';

const PeriodoFechamentoAbertura = () => {
  const [listaCalendarioEscolar, setListaCalendarioEscolar] = useState([]);
  const [listaDres, setListaDres] = useState([]);
  const [listaUes, setListaUes] = useState([]);
  const [
    calendarioEscolarSelecionado,
    setCalendarioEscolarSelecionado,
  ] = useState('');
  const [dreSelecionada, setDreSelecionada] = useState('');
  const [ueSlecionada, setUeSelecionada] = useState('');
  const [ehTipoCalendarioAnual, setEhTipoCalendarioAnual] = useState(true);
  const [desabilitaCampos, setDesabilitaCampos] = useState(false);

  const chaveBimestre = {
    primeiroInicio: 'primeiroBimestreDataInicial',
    primeiroFinal: 'primeiroBimestreDataFinal',
    segundoInicio: 'segundoBimestreDataInicial',
    segundoFinal: 'segundoBimestreDataFinal',
    terceiroInicio: 'terceiroBimestreDataInicial',
    terceiroFinal: 'terceiroBimestreDataFinal',
    quartoInicio: 'quartoBimestreDataInicial',
    quartoFinal: 'quartoBimestreDataFinal',
  };
  const valoresFormInicial = {
    [chaveBimestre.primeiroInicio]: '',
    [chaveBimestre.primeiroFinal]: '',
    [chaveBimestre.segundoInicio]: '',
    [chaveBimestre.segundoFinal]: '',
    [chaveBimestre.terceiroInicio]: '',
    [chaveBimestre.terceiroFinal]: '',
    [chaveBimestre.quartoInicio]: '',
    [chaveBimestre.quartoFinal]: '',
  };
  const [valoresIniciais, setValoresIniciais] = useState(valoresFormInicial);
  const [validacoes, setValidacoes] = useState();
  const [carregandoTipos, setCarregandoTipos] = useState(false);
  const [carregandoDres, setCarregandoDres] = useState(false);
  const [carregandoUes, setCarregandoUes] = useState(false);

  useEffect(() => {
    async function consultaTipos() {
      setCarregandoTipos(true);
      const listaTipo = await api.get('v1/calendarios/tipos');
      if (listaTipo && listaTipo.data && listaTipo.data.length) {
        listaTipo.data.map(item => {
          item.id = String(item.id);
          item.descricaoTipoCalendario = `${item.anoLetivo} - ${item.nome} - ${item.descricaoPeriodo}`;
        });
        setListaCalendarioEscolar(listaTipo.data);
      } else {
        setListaCalendarioEscolar([]);
      }
    }
    consultaTipos();
    setCarregandoTipos(false);
  }, []);

  const onChangeCamposData = () => { };

  const onchangeCalendarioEscolar = (id, form) => {
    const tipoSelecionado = listaCalendarioEscolar.find(item => item.id == id);

    if (tipoSelecionado && tipoSelecionado.periodo == periodo.Anual) {
      setEhTipoCalendarioAnual(true);
    } else {
      setEhTipoCalendarioAnual(false);
    }
    setCalendarioEscolarSelecionado(id);
  };

  const onChangeDre = () => { };

  const onChangeUe = () => { };

  const onClickVoltar = () => { };

  const onClickCancelar = form => { };

  const validaAntesDoSubmit = form => { };

  const onSubmit = form => { };

  const criaBimestre = (form, descricao, chaveDataInicial, chaveDataFinal) => {
    return (
      <div className="row">
        <div className="col-md-6 mb-2">
          <CaixaBimestre>
            <BoxTextoBimetre>{descricao}</BoxTextoBimetre>
          </CaixaBimestre>
        </div>
        <div className="col-md-3 mb-2">
          <CampoData
            form={form}
            placeholder="Início do Bimestre"
            formatoData="DD/MM/YYYY"
            name={chaveDataInicial}
            onChange={onChangeCamposData}
            desabilitado={desabilitaCampos}
          />
        </div>
        <div className="col-md-3 mb-2">
          <CampoData
            form={form}
            placeholder="Fim do Bimestre"
            formatoData="DD/MM/YYYY"
            name={chaveDataFinal}
            onChange={onChangeCamposData}
            desabilitado={desabilitaCampos}
          />
        </div>
      </div>
    );
  };

  return (
    <>
      <Cabecalho pagina="Período de Fechamento (Abertura)" />
      <Card>
        <Formik
          enableReinitialize
          initialValues={valoresIniciais}
          validationSchema={validacoes}
          onSubmit={values => onSubmit(values)}
          validateOnChange
          validateOnBlur
        >
          {form => (
            <Form className="col-md-12">
              <div className="row">
                <div className="col-md-12 d-flex justify-content-end pb-4">
                  <Button
                    label="Voltar"
                    icon="arrow-left"
                    color={Colors.Azul}
                    border
                    className="mr-3"
                    onClick={onClickVoltar}
                  />
                  <Button
                    label="Cancelar"
                    color={Colors.Roxo}
                    border
                    bold
                    className="mr-3"
                    onClick={() => onClickCancelar(form)}
                  />
                  <Button
                    label="Cadastrar"
                    color={Colors.Roxo}
                    border
                    bold
                    onClick={() => validaAntesDoSubmit(form)}
                  />
                </div>
                <div className="col-md-12 pb-2">
                  <Loader loading={carregandoTipos} tip="">
                    <div style={{ maxWidth: '250px' }}>
                      <SelectComponent
                        name="tipoCalendario"
                        id="tipoCalendario"
                        placeholder="Tipo de Calendário Escolar"
                        lista={listaCalendarioEscolar}
                        valueOption="id"
                        valueText="descricaoTipoCalendario"
                        onChange={id => onchangeCalendarioEscolar(id, form)}
                        valueSelect={calendarioEscolarSelecionado}
                        disabled={false}
                      />
                    </div>
                  </Loader>
                </div><br />
                <div className="col-md-6 pb-2">
                  <Loader loading={carregandoDres} tip="">
                    <SelectComponent
                      name="dre"
                      id="dre"
                      placeholder="Diretoria Regional de Educação (DRE)"
                      lista={listaDres}
                      valueOption="valor"
                      valueText="desc"
                      onChange={id => onChangeDre()}
                      valueSelect={dreSelecionada}
                      disabled={false}
                    />
                  </Loader>
                </div>
                <div className="col-md-6 pb-2">
                  <Loader loading={carregandoUes} tip="">
                    <SelectComponent
                      name="ue"
                      id="ue"
                      placeholder="Unidade Escolar (UE)"
                      lista={listaUes}
                      valueOption="value"
                      valueText="desc"
                      onChange={id => onChangeUe()}
                      valueSelect={ueSlecionada}
                      disabled={false}
                    />
                  </Loader>
                </div>
              </div>
              {listaCalendarioEscolar &&
                listaCalendarioEscolar.length &&
                calendarioEscolarSelecionado ? (
                  <>
                    {criaBimestre(
                      form,
                      '1 ° Bimestre',
                      chaveBimestre.primeiroInicio,
                      chaveBimestre.primeiroFinal
                    )}
                    {criaBimestre(
                      form,
                      '2 ° Bimestre',
                      chaveBimestre.segundoInicio,
                      chaveBimestre.segundoFinal
                    )}

                    {ehTipoCalendarioAnual ? (
                      <>
                        {criaBimestre(
                          form,
                          '3 ° Bimestre',
                          chaveBimestre.terceiroInicio,
                          chaveBimestre.terceiroFinal
                        )}
                        {criaBimestre(
                          form,
                          '4 ° Bimestre',
                          chaveBimestre.quartoInicio,
                          chaveBimestre.quartoFinal
                        )}
                      </>
                    ) : (
                        ''
                      )}
                  </>
                ) : (
                  ''
                )}
            </Form>
          )}
        </Formik>
      </Card>
    </>
  );
};

export default PeriodoFechamentoAbertura;
