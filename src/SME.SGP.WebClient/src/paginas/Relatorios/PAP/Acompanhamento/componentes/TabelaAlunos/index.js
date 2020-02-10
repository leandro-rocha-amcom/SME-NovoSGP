import React, { useEffect } from 'react';
import t from 'prop-types';
import shortid from 'shortid';

// Componentes
import IconeStatus from './componentes/IconeStatus';
import SelectRespostas from './componentes/SelectRespostas';

// Estilos
import { Tabela, ContainerTabela } from './styles';

function TabelaAlunos({ alunos, respostas, objetivoAtivo, onChangeResposta }) {
  return (
    <ContainerTabela>
      <Tabela>
        <thead>
          <tr>
            <td colSpan="2">Estudante</td>
            <td>Concluído</td>
            <td>Turma regular</td>
            <td>Respostas</td>
          </tr>
        </thead>
        <tbody>
          {alunos.length > 1 ? (
            alunos.map((aluno, key) => (
              <tr key={shortid.generate()}>
                <td>{aluno.numeroChamada}</td>
                <td>{aluno.nome}</td>
                <td>
                  <IconeStatus status={aluno.concluido} />
                </td>
                <td>{aluno.turma}</td>
                <td id={`resposta-${key}`}>
                  <div className="opcaoSelect">
                    <SelectRespostas
                      aluno={aluno}
                      objetivoAtivo={objetivoAtivo}
                      respostas={respostas}
                      onChangeResposta={onChangeResposta}
                      containerVinculoId={`resposta-${key}`}
                    />
                  </div>
                </td>
              </tr>
            ))
          ) : (
            <tr className="semConteudo">
              <td>Sem dados</td>
            </tr>
          )}
        </tbody>
      </Tabela>
    </ContainerTabela>
  );
}

TabelaAlunos.propTypes = {
  alunos: t.oneOfType([t.array]),
};

TabelaAlunos.defaultProps = {
  alunos: [],
};

export default TabelaAlunos;