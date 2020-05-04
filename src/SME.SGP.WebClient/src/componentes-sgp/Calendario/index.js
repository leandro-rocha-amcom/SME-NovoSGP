import React, { useReducer, useCallback, useEffect } from 'react';
import t from 'prop-types';

// Componentes
import { Loader } from '~/componentes';

// Componentes internos
import Mes from './componentes/Mes';
import MesCompleto from './componentes/MesCompleto';

// Estilos
import { FundoCinza } from './styles';

// Reducer
import Reducer, { estadoInicial } from './reducer';

// Actions
import { selecionarMes, selecionarDia } from './reducer/actions';

const obterMesesPermitidos = numeroMes => {
  if (numeroMes <= 4) return [1, 2, 3, 4];
  if (numeroMes > 4 && numeroMes <= 8) return [5, 6, 7, 8];
  return [9, 10, 11, 12];
};

function Calendario({
  eventos,
  onClickMes,
  onClickDia,
  carregandoCalendario,
  carregandoMes,
  carregandoDia,
}) {
  const [estado, disparar] = useReducer(Reducer, estadoInicial);

  const onClickMesHandler = useCallback(
    mes => {
      disparar(selecionarMes(mes));
      if (!mes.estaAberto) onClickMes(mes);
    },
    [onClickMes]
  );

  const onClickDiaHandler = useCallback(
    dia => {
      disparar(selecionarDia({ diaSelecionado: dia }));
      onClickDia(dia);
    },
    [onClickDia]
  );

  return (
    <Loader loading={carregandoCalendario}>
      <FundoCinza>
        {estado.meses &&
          estado.meses.map(item => (
            <React.Fragment key={item.numeroMes}>
              <Mes mes={item} onClickMes={() => onClickMesHandler(item)} />
              {item.numeroMes % 4 === 0 && (
                <MesCompleto
                  eventos={eventos}
                  mes={estado.meses.filter(x => x.estaAberto)[0]}
                  mesesPermitidos={obterMesesPermitidos(item.numeroMes)}
                  diaSelecionado={estado.diaSelecionado}
                  onClickDia={dia => onClickDiaHandler(dia)}
                  carregandoDia={carregandoDia}
                  carregandoMes={carregandoMes}
                />
              )}
            </React.Fragment>
          ))}
      </FundoCinza>
    </Loader>
  );
}

Calendario.propTypes = {
  eventos: t.oneOfType([t.any]),
  onClickMes: t.func,
  onClickDia: t.func,
  carregandoCalendario: t.bool,
  carregandoMes: t.bool,
  carregandoDia: t.bool,
};

Calendario.defaultProps = {
  eventos: [],
  onClickMes: () => {},
  onClickDia: () => {},
  carregandoCalendario: false,
  carregandoMes: false,
  carregandoDia: false,
};

export default Calendario;
