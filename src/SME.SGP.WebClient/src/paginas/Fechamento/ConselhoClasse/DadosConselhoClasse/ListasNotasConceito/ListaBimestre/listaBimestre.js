import PropTypes from 'prop-types';
import React from 'react';
import shortid from 'shortid';
import notasConceitos from '~/dtos/notasConceitos';
import CampoConceito from '../CamposNotaConceito/campoConceito';
import CampoNota from '../CamposNotaConceito/campoNota';
import {
  BarraLateralBordo,
  BarraLateralVerde,
  Lista,
} from '../listasNotasConceitos.css';
import LinhaJustificativa from '../../Justificativa/LinhaJustificativa/LinhaJustificativa';

const ListaBimestre = props => {
  const { dadosLista, tipoNota, listaTiposConceitos, mediaAprovacao } = props;

  const descricaoTipoNota =
    tipoNota === notasConceitos.Notas ? 'Nota' : 'Conceito';

  const descricaoGrupoMatriz =
    dadosLista && dadosLista.grupoMatriz
      ? dadosLista.grupoMatriz
      : 'Componente';

  const alturaLinhaMesclada =
    dadosLista &&
    dadosLista.componenteRegencia &&
    dadosLista.componenteRegencia.componentesCurriculares &&
    dadosLista.componenteRegencia.componentesCurriculares.length
      ? dadosLista.componenteRegencia.componentesCurriculares.length * 2
      : 0;

  const montaCampoPosConselho = (
    id,
    notaPosConselho,
    idCampo,
    codigoComponenteCurricular
  ) => {
    switch (Number(tipoNota)) {
      case Number(notasConceitos.Notas):
        return (
          <CampoNota
            id={id}
            notaPosConselho={notaPosConselho}
            idCampo={idCampo}
            codigoComponenteCurricular={String(codigoComponenteCurricular)}
            mediaAprovacao={mediaAprovacao}
          />
        );
      case Number(notasConceitos.Conceitos):
        return (
          <CampoConceito
            notaPosConselho={notaPosConselho}
            listaTiposConceitos={listaTiposConceitos}
            id={id}
            idCampo={idCampo}
            codigoComponenteCurricular={codigoComponenteCurricular}
          />
        );
      default:
        return '';
    }
  };

  const obterValorNotaConceito = valor => {
    const ehNota = Number(notasConceitos.Notas) === tipoNota;
    if (valor && !ehNota && listaTiposConceitos && listaTiposConceitos.length) {
      const conceito = listaTiposConceitos.find(item => item.id == valor);
      return conceito ? conceito.valor : '';
    }
    return valor || '';
  };

  const montarValoresNotasConceitos = item => {
    const notaFechamento =
      item && item.notasFechamentos && item.notasFechamentos[0]
        ? item.notasFechamentos[0].notaConceito
        : '';
    return (
      <div
        className="input-notas-conceitos"
        style={{ display: 'inline-block' }}
      >
        <span>{obterValorNotaConceito(notaFechamento)}</span>
      </div>
    );
  };

  return (
    <Lista className="pl-2 pr-2">
      <div className="table-responsive pl-2 pr-2">
        <table className="table mt-4">
          <thead className="tabela-conselho-thead">
            <tr>
              <th
                colSpan="2"
                className="coluna-disciplina sombra-direita"
                style={{ width: '250px', paddingLeft: '27px' }}
              >
                {descricaoGrupoMatriz}
              </th>
              <th>{descricaoTipoNota}</th>
              <th>{`${descricaoTipoNota} pós-conselho`}</th>
              <th>Aulas</th>
              <th>Faltas</th>
              <th style={{ width: '100px' }}>Ausências Compensadas</th>
              <th>%Freq.</th>
            </tr>
          </thead>
          <tbody className="tabela-conselho-tbody">
            {dadosLista &&
              dadosLista.componentesCurriculares &&
              dadosLista.componentesCurriculares.map((item, index) => {
                return (
                  <>
                    <tr key={shortid.generate()}>
                      <BarraLateralVerde />
                      <td
                        className="coluna-disciplina sombra-direita"
                        style={{
                          width: '250px',
                          textAlign: 'left',
                          paddingLeft: '20px',
                        }}
                      >
                        {item.nome}
                      </td>
                      <td className="text-center">
                        {montarValoresNotasConceitos(item)}
                      </td>
                      <td>
                        {montaCampoPosConselho(
                          item.notaPosConselho.id,
                          item.notaPosConselho.nota,
                          `${descricaoGrupoMatriz} ${index} componente`,
                          item.codigoComponenteCurricular
                        )}
                      </td>
                      <td>{item.quantidadeAulas}</td>
                      <td>{item.faltas}</td>
                      <td>{item.ausenciasCompensadas}</td>
                      <td>{item.frequencia}%</td>
                    </tr>
                    <LinhaJustificativa
                      idCampo={`${descricaoGrupoMatriz} ${index} componente`}
                      ehRegencia={false}
                    />
                  </>
                );
              })}
            {dadosLista &&
              dadosLista.componenteRegencia &&
              dadosLista.componenteRegencia.componentesCurriculares &&
              dadosLista.componenteRegencia.componentesCurriculares.map(
                (item, index) => {
                  return (
                    <>
                      <tr key={shortid.generate()}>
                        <BarraLateralBordo />
                        <td
                          className="coluna-disciplina sombra-direita"
                          style={{ textAlign: 'left', paddingLeft: '20px' }}
                        >
                          {item.nome}
                        </td>
                        <td>{montarValoresNotasConceitos(item)}</td>
                        <td>
                          {montaCampoPosConselho(
                            item.notaPosConselho.id,
                            item.notaPosConselho.nota,
                            `${descricaoGrupoMatriz} ${index} regencia`,
                            item.codigoComponenteCurricular
                          )}
                        </td>
                        {index === 0 ? (
                          <td rowSpan={alturaLinhaMesclada}>
                            {dadosLista.componenteRegencia.quantidadeAulas}
                          </td>
                        ) : null}
                        {index === 0 ? (
                          <td rowSpan={alturaLinhaMesclada}>
                            {dadosLista.componenteRegencia.faltas}
                          </td>
                        ) : null}
                        {index === 0 ? (
                          <td rowSpan={alturaLinhaMesclada}>
                            {dadosLista.componenteRegencia.ausenciasCompensadas}
                          </td>
                        ) : null}
                        {index === 0 ? (
                          <td rowSpan={alturaLinhaMesclada}>
                            {dadosLista.componenteRegencia.frequencia}%
                          </td>
                        ) : null}
                      </tr>
                      <LinhaJustificativa
                        idCampo={`${descricaoGrupoMatriz} ${index} regencia`}
                        ehRegencia
                      />
                    </>
                  );
                }
              )}
          </tbody>
        </table>
      </div>
    </Lista>
  );
};

ListaBimestre.propTypes = {
  dadosLista: PropTypes.oneOfType([PropTypes.object]),
  tipoNota: PropTypes.oneOfType([PropTypes.any]),
  listaTiposConceitos: PropTypes.oneOfType([PropTypes.array]),
  mediaAprovacao: PropTypes.number,
};

ListaBimestre.defaultProps = {
  dadosLista: {},
  tipoNota: 0,
  listaTiposConceitos: [],
  mediaAprovacao: 5,
};

export default ListaBimestre;
