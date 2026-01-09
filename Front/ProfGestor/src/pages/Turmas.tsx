import { useEffect, useState } from 'react';
import { AuthenticatedLayout } from '../components/Layout/AuthenticatedLayout';
import { Loading } from '../components/UI/Loading';
import { ErrorMessage } from '../components/UI/ErrorMessage';
import { TurmaForm } from '../components/Turmas/TurmaForm';
import { TurmaDetalhesModal } from '../components/Turmas/TurmaDetalhesModal';
import { turmaService, type Turma } from '../services/turmaService';
import { showError, showInfo } from '../utils/toast';
import './Turmas.css';

export const Turmas = () => {
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [turmaDetalhesId, setTurmaDetalhesId] = useState<number | null>(null);

  useEffect(() => {
    const fetchTurmas = async () => {
      try {
        setLoading(true);
        const data = await turmaService.getAll();
        setTurmas(data);
        setError(null);
      } catch (err: any) {
        console.error('Erro ao carregar turmas:', err);
        setError('Erro ao carregar turmas. Tente novamente mais tarde.');
      } finally {
        setLoading(false);
      }
    };

    fetchTurmas();
  }, []);

  const handleDelete = async (id: number, nome: string) => {
    if (!window.confirm(`Tem certeza que deseja excluir a turma "${nome}"?`)) {
      return;
    }

    try {
      await turmaService.delete(id);
      setTurmas(turmas.filter(t => t.id !== id));
    } catch (err: any) {
      console.error('Erro ao excluir turma:', err);
      showError('Erro ao excluir turma. Tente novamente.');
    }
  };

  const handleFormSuccess = async () => {
    setShowForm(false);
    // Recarregar lista de turmas
    try {
      const data = await turmaService.getAll();
      setTurmas(data);
    } catch (err: any) {
      console.error('Erro ao recarregar turmas:', err);
    }
  };

  const handleFormCancel = () => {
    setShowForm(false);
  };

  const formatTurno = (turno: string): string => {
    const turnos: { [key: string]: string } = {
      'Matutino': 'Matutino',
      'Vespertino': 'Vespertino',
      'Noturno': 'Noturno',
    };
    return turnos[turno] || turno;
  };

  return (
    <AuthenticatedLayout>
      <div className="turmas-content">
        {/* Cabeçalho da página */}
        <div className="page-header">
          <div className="page-title-section">
            <h1 className="page-title">Minhas Turmas</h1>
            <p className="page-subtitle">
              Gerencie suas turmas e visualize informações dos alunos
            </p>
          </div>
          <button 
            className="btn-nova-turma" 
            onClick={() => setShowForm(!showForm)}
          >
            <span className="btn-icon">+</span>
            Nova Turma
          </button>
        </div>

        {error && <ErrorMessage message={error} onDismiss={() => setError(null)} />}
        {loading && <Loading message="Carregando turmas..." />}

        {/* Formulário de cadastro */}
        {showForm && (
          <TurmaForm 
            onSuccess={handleFormSuccess}
            onCancel={handleFormCancel}
          />
        )}

        {/* Modal de detalhes da turma */}
        {turmaDetalhesId !== null && (
          <TurmaDetalhesModal
            turmaId={turmaDetalhesId}
            onClose={() => setTurmaDetalhesId(null)}
          />
        )}

        {/* Grid de turmas */}
        {!loading && !error && (
          <div className="turmas-grid">
            {turmas.length > 0 ? (
              turmas.map((turma) => (
                <div key={turma.id} className="turma-card">
                  <div className="turma-card-header">
                    <h3 className="turma-nome">{turma.nome} - {formatTurno(turma.turno)}</h3>
                    <span className="semestre-badge">{turma.semestre}º Semestre</span>
                  </div>
                  <div className="turma-card-body">
                    <div className="turma-info-item">
                      <span className="info-icon">📖</span>
                      <span className="info-text">{turma.disciplinaNome}</span>
                    </div>
                    <div className="turma-info-item">
                      <span className="info-icon">👥</span>
                      <span className="info-text">{turma.totalAlunos || turma.qtdAlunos} alunos</span>
                    </div>
                  </div>
                  <div className="turma-card-actions">
                    <button 
                      className="btn-ver-detalhes"
                      onClick={() => setTurmaDetalhesId(turma.id)}
                    >
                      <span className="btn-icon">👁️</span>
                      Ver Detalhes
                    </button>
                    <button 
                      className="btn-delete"
                      onClick={() => handleDelete(turma.id, turma.nome)}
                      title="Excluir turma"
                    >
                      🗑️
                    </button>
                  </div>
                </div>
              ))
            ) : (
              <div className="empty-state">
                <p>Nenhuma turma cadastrada ainda.</p>
                <button className="btn-nova-turma" onClick={() => showInfo('Funcionalidade em desenvolvimento')}>
                  <span className="btn-icon">+</span>
                  Criar Primeira Turma
                </button>
              </div>
            )}
          </div>
        )}
      </div>
    </AuthenticatedLayout>
  );
};

