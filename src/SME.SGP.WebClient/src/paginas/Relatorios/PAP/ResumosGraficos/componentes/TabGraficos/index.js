import React from 'react';
import t from 'prop-types';

// Componentes
import { BarraNavegacao, Base, Graficos } from '~/componentes';
import EixoObjetivo from './componentes/EixoObjetivo';

// Estilos
import { Linha } from '~/componentes/EstilosGlobais';

function TabGraficos({ dados }) {
  const dadosBackend = [
    {
      key: '0',
      DescricaoFrequencia: 'Frequente',
      TipoDado: 'Quantidade',
      Cor: Base.Laranja,
      '3C': 11,
      '4C': 15,
      '4E': 20,
      '5C': 25,
      '6C': 25,
      '6B': 25,
      Total: 36,
    },
    {
      key: '1',
      DescricaoFrequencia: 'Frequente',
      TipoDado: 'Porcentagem',
      Cor: Base.Laranja,
      '3C': 11,
      '4C': 15,
      '4E': 20,
      '5C': 25,
      '6C': 25,
      '6B': 25,
      Total: 36,
    },
    {
      key: '2',
      DescricaoFrequencia: 'Pouco frequente',
      TipoDado: 'Quantidade',
      Cor: Base.Vermelho,
      '3C': 11,
      '4C': 15,
      '4E': 20,
      '5C': 25,
      '6C': 25,
      '6B': 25,
      Total: 36,
    },
    {
      key: '3',
      DescricaoFrequencia: 'Pouco frequente',
      TipoDado: 'Porcentagem',
      Cor: Base.Vermelho,
      '3C': 11,
      '4C': 15,
      '4E': 20,
      '5C': 25,
      '6C': 25,
      '6B': 25,
      Total: 36,
    },
  ];

  return (
    <>
      <Linha style={{ marginBottom: '8px' }}>
        <BarraNavegacao />
      </Linha>
      <Linha style={{ marginBottom: '35px' }}>
        <EixoObjetivo />
      </Linha>
      <Linha style={{ marginBottom: '35px', textAlign: 'center' }}>
        <h3>Quantidade</h3>
        {dadosBackend && (
          <div style={{ height: 400 }}>
            <Graficos.Barras
              dados={dadosBackend.filter(x => x.TipoDado === 'Quantidade')}
              indice="DescricaoFrequencia"
              chaves={['3C', '4C', '4E', '5C', '6C', '6B']}
              legendaBaixo="teste"
              legendaEsquerda="teste2"
            />
          </div>
        )}
      </Linha>
      <Linha style={{ marginBottom: '35px', textAlign: 'center' }}>
        <h3>Porcentagem</h3>
        {dadosBackend && (
          <div style={{ height: 400 }}>
            <Graficos.Barras
              dados={dadosBackend.filter(x => x.TipoDado === 'Porcentagem')}
              indice="DescricaoFrequencia"
              chaves={['3C', '4C', '4E', '5C', '6C', '6B']}
              legendaBaixo="teste"
              legendaEsquerda="teste2"
            />
          </div>
        )}
      </Linha>
    </>
  );
}

TabGraficos.propTypes = {
  dados: t.oneOfType([t.any]),
};

TabGraficos.defaultProps = {
  dados: [],
};

export default TabGraficos;
