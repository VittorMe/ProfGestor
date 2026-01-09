import { useEffect, useState } from 'react';
import { turmaService, type Turma } from '../../services/turmaService';
import { alunoService, type Aluno } from '../../services/alunoService';
import { aulaService, type Aula } from '../../services/aulaService';
import { Loading } from '../UI/Loading';
import { ErrorMessage } from '../UI/ErrorMessage';
import './TurmaDetalhesModal.css';

const formatDateOnly = (dateString: string): string => {
  const date = new Date(dateString);
  return date.toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  });
};

interface TurmaDetalhesModalProps {
  turmaId: number;
  onClose: () => void;
}

export const TurmaDetalhesModal = ({ turmaId, onClose }: TurmaDetalhesModalProps) => {
  const [turma, setTurma] = useState<Turma | null>(null);
  const [alunos, setAlunos] = useState<Aluno[]>([]);
  const [aulas, setAulas] = useState<Aula[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        setError(null);

        // Buscar dados em paralelo
        const [turmaData, alunosData, aulasData] = await Promise.all([
          turmaService.getById(turmaId),
          alunoService.getByTurmaId(turmaId),
          aulaService.getByTurmaId(turmaId),
        ]);

        setTurma(turmaData);
        setAlunos(alunosData);
        setAulas(aulasData);
      } catch (err: any) {
        console.error('Erro ao carregar detalhes da turma:', err);
        setError('Erro ao carregar detalhes da turma. Tente novamente.');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [turmaId]);

  const formatTurno = (turno: string): string => {
    const turnos: { [key: string]: string } = {
      'Matutino': 'Matutino',
      'Vespertino': 'Vespertino',
      'Noturno': 'Noturno',
    };
    return turnos[turno] || turno;
  };

  // Ordenar aulas por data (mais recentes primeiro)
  const aulasOrdenadas = [...aulas].sort((a, b) => {
    const dataA = new Date(a.data);
    const dataB = new Date(b.data);
    return dataB.getTime() - dataA.getTime();
  });

  // Pegar últimas 5 aulas
  const ultimasAulas = aulasOrdenadas.slice(0, 5);

  if (loading) {
    return (
      <div className="modal-overlay" onClick={onClose}>
        <div className="modal-content" onClick={(e) => e.stopPropagation()}>
          <Loading message="Carregando detalhes da turma..." />
        </div>
      </div>
    );
  }

  if (error || !turma) {
    return (
      <div className="modal-overlay" onClick={onClose}>
        <div className="modal-content" onClick={(e) => e.stopPropagation()}>
          <div className="modal-header">
            <h2>Detalhes da Turma</h2>
            <button className="modal-close" onClick={onClose}>×</button>
          </div>
          <ErrorMessage message={error || 'Turma não encontrada'} onDismiss={onClose} />
        </div>
      </div>
    );
  }

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content turma-detalhes-modal" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Detalhes da Turma</h2>
          <button className="modal-close" onClick={onClose}>×</button>
        </div>

        <div className="modal-body">
          {/* Informações Gerais */}
          <section className="detalhes-section">
            <h3 className="section-title">Informações Gerais</h3>
            <div className="info-grid">
              <div className="info-item">
                <span className="info-label">Nome:</span>
                <span className="info-value">{turma.nome}</span>
              </div>
              <div className="info-item">
                <span className="info-label">Disciplina:</span>
                <span className="info-value">{turma.disciplinaNome}</span>
              </div>
              <div className="info-item">
                <span className="info-label">Ano Letivo:</span>
                <span className="info-value">{turma.anoLetivo}</span>
              </div>
              <div className="info-item">
                <span className="info-label">Semestre:</span>
                <span className="info-value">{turma.semestre}º Semestre</span>
              </div>
              <div className="info-item">
                <span className="info-label">Turno:</span>
                <span className="info-value">{formatTurno(turma.turno)}</span>
              </div>
              <div className="info-item">
                <span className="info-label">Total de Alunos:</span>
                <span className="info-value">{turma.totalAlunos || turma.qtdAlunos}</span>
              </div>
            </div>
          </section>

          {/* Lista de Alunos */}
          <section className="detalhes-section">
            <h3 className="section-title">
              Alunos ({alunos.length})
            </h3>
            {alunos.length > 0 ? (
              <div className="alunos-list">
                <table className="alunos-table">
                  <thead>
                    <tr>
                      <th>Matrícula</th>
                      <th>Nome</th>
                    </tr>
                  </thead>
                  <tbody>
                    {alunos.map((aluno) => (
                      <tr key={aluno.id}>
                        <td>{aluno.matricula}</td>
                        <td>{aluno.nome}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : (
              <p className="empty-message">Nenhum aluno cadastrado nesta turma.</p>
            )}
          </section>

          {/* Últimas Aulas */}
          <section className="detalhes-section">
            <h3 className="section-title">
              Últimas Aulas ({aulas.length} total)
            </h3>
            {ultimasAulas.length > 0 ? (
              <div className="aulas-list">
                {ultimasAulas.map((aula) => (
                  <div key={aula.id} className="aula-item">
                    <div className="aula-data">
                      <span className="aula-data-text">{formatDateOnly(aula.data)}</span>
                      <span className="aula-periodo">{aula.periodo}</span>
                    </div>
                    {aula.temFrequenciaRegistrada && (
                      <span className="frequencia-badge">✓ Frequência registrada</span>
                    )}
                  </div>
                ))}
                {aulas.length > 5 && (
                  <p className="more-items">... e mais {aulas.length - 5} aula(s)</p>
                )}
              </div>
            ) : (
              <p className="empty-message">Nenhuma aula registrada ainda.</p>
            )}
          </section>
        </div>

        <div className="modal-footer">
          <button className="btn-close-modal" onClick={onClose}>
            Fechar
          </button>
        </div>
      </div>
    </div>
  );
};

