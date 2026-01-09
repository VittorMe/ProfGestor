import { useEffect, useState } from 'react';
import { AuthenticatedLayout } from '../components/Layout';
import { Loading } from '../components/UI/Loading';
import { ErrorMessage } from '../components/UI/ErrorMessage';
import { avaliacaoService, type Avaliacao } from '../services/avaliacaoService';
import { disciplinaService, type Disciplina } from '../services/disciplinaService';
import { AvaliacaoCard } from '../components/Avaliacoes/AvaliacaoCard';
import { AvaliacaoForm } from '../components/Avaliacoes/AvaliacaoForm';
import './Avaliacoes.css';

export const Avaliacoes = () => {
  const [avaliacoes, setAvaliacoes] = useState<Avaliacao[]>([]);
  const [disciplinas, setDisciplinas] = useState<Disciplina[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filtroDisciplina, setFiltroDisciplina] = useState<number | ''>('');
  const [showForm, setShowForm] = useState(false);
  const [editingAvaliacao, setEditingAvaliacao] = useState<Avaliacao | null>(null);

  useEffect(() => {
    const fetchAvaliacoesAndDisciplinas = async () => {
      try {
        setLoading(true);
        setError(null);

        const [avaliacoesData, disciplinasData] = await Promise.all([
          avaliacaoService.getAll(),
          disciplinaService.getAll(),
        ]);

        setAvaliacoes(avaliacoesData);
        setDisciplinas(disciplinasData);
      } catch (err: any) {
        console.error('Erro ao carregar avaliações ou disciplinas:', err);
        setError(err.response?.data?.error || 'Erro ao carregar dados. Tente novamente.');
      } finally {
        setLoading(false);
      }
    };

    fetchAvaliacoesAndDisciplinas();
  }, []);

  useEffect(() => {
    const fetchAvaliacoes = async () => {
      try {
        setLoading(true);
        setError(null);

        const disciplinaId = filtroDisciplina ? Number(filtroDisciplina) : undefined;
        const avaliacoesData = await avaliacaoService.getAll(disciplinaId);
        setAvaliacoes(avaliacoesData);
      } catch (err: any) {
        console.error('Erro ao carregar avaliações:', err);
        setError(err.response?.data?.error || 'Erro ao carregar avaliações. Tente novamente.');
      } finally {
        setLoading(false);
      }
    };

    fetchAvaliacoes();
  }, [filtroDisciplina]);

  const handleFiltroChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setFiltroDisciplina(e.target.value === '' ? '' : Number(e.target.value));
  };

  const handleNewAvaliacao = () => {
    setEditingAvaliacao(null);
    setShowForm(true);
  };

  // Funções de edição e exclusão podem ser implementadas no futuro se necessário

  const handleFormSuccess = async () => {
    setShowForm(false);
    setEditingAvaliacao(null);
    
    try {
      const disciplinaId = filtroDisciplina ? Number(filtroDisciplina) : undefined;
      const avaliacoesData = await avaliacaoService.getAll(disciplinaId);
      setAvaliacoes(avaliacoesData);
    } catch (err: any) {
      console.error('Erro ao recarregar avaliações:', err);
    }
  };

  const handleFormCancel = () => {
    setShowForm(false);
    setEditingAvaliacao(null);
  };

  // A navegação é feita diretamente no componente AvaliacaoCard

  return (
    <AuthenticatedLayout>
      <div className="avaliacoes-content">
        <div className="avaliacoes-header">
          <div className="header-text">
            <h1>Avaliações e Notas</h1>
            <p>Gerencie avaliações, gabaritos e lançamento de notas</p>
          </div>
          <button className="btn-nova-avaliacao" onClick={handleNewAvaliacao}>
            <span className="btn-icon">+</span>
            Nova Avaliação
          </button>
        </div>

        <div className="avaliacoes-filters">
          <select
            className="filter-select"
            value={filtroDisciplina}
            onChange={handleFiltroChange}
          >
            <option value="">Todas as disciplinas</option>
            {disciplinas.map(disciplina => (
              <option key={disciplina.id} value={disciplina.id}>
                {disciplina.nome}
              </option>
            ))}
          </select>
        </div>

        {error && <ErrorMessage message={error} onDismiss={() => setError(null)} />}
        {loading && <Loading message="Carregando avaliações..." />}

        {showForm && (
          <AvaliacaoForm
            onSuccess={handleFormSuccess}
            onCancel={handleFormCancel}
            disciplinas={disciplinas}
            avaliacaoToEdit={editingAvaliacao}
          />
        )}

        {!loading && !error && (
          <div className="avaliacoes-grid">
            {avaliacoes.length > 0 ? (
              avaliacoes.map(avaliacao => (
                <AvaliacaoCard
                  key={avaliacao.id}
                  avaliacao={avaliacao}
                />
              ))
            ) : (
              <div className="empty-state">
                <p>Nenhuma avaliação encontrada.</p>
                <button className="btn-nova-avaliacao" onClick={handleNewAvaliacao}>
                  <span className="btn-icon">+</span>
                  Criar Primeira Avaliação
                </button>
              </div>
            )}
          </div>
        )}
      </div>
    </AuthenticatedLayout>
  );
};

