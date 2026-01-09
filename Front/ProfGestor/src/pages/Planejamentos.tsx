import { useEffect, useState } from 'react';
import { AuthenticatedLayout } from '../components/Layout/AuthenticatedLayout';
import { Loading } from '../components/UI/Loading';
import { ErrorMessage } from '../components/UI/ErrorMessage';
import { PlanejamentoCard } from '../components/Planejamentos/PlanejamentoCard';
import { PlanejamentoForm } from '../components/Planejamentos/PlanejamentoForm';
import { PlanejamentoDetalhesModal } from '../components/Planejamentos/PlanejamentoDetalhesModal';
import { planejamentoService, type Planejamento } from '../services/planejamentoService';
import { disciplinaService, type Disciplina } from '../services/disciplinaService';
import { turmaService } from '../services/turmaService';
import { showError } from '../utils/toast';
import './Planejamentos.css';

export const Planejamentos = () => {
  const [planejamentos, setPlanejamentos] = useState<Planejamento[]>([]);
  const [disciplinas, setDisciplinas] = useState<Disciplina[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [detalhesId, setDetalhesId] = useState<number | null>(null);
  
  // Filtros
  const [searchTerm, setSearchTerm] = useState('');
  const [disciplinaFiltro, setDisciplinaFiltro] = useState<number | ''>('');
  const [apenasFavoritos, setApenasFavoritos] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        setError(null);

        // Buscar disciplinas das turmas do professor
        const turmas = await turmaService.getAll();
        const disciplinaIds = [...new Set(turmas.map(t => t.disciplinaId))];
        
        // Buscar detalhes das disciplinas
        const todasDisciplinas = await disciplinaService.getAll();
        const disciplinasFiltradas = todasDisciplinas.filter(d => disciplinaIds.includes(d.id));
        setDisciplinas(disciplinasFiltradas);

        // Buscar planejamentos
        await loadPlanejamentos();
      } catch (err: any) {
        console.error('Erro ao carregar dados:', err);
        setError('Erro ao carregar dados. Tente novamente mais tarde.');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  useEffect(() => {
    // Recarregar planejamentos quando filtros mudarem
    if (!loading) {
      loadPlanejamentos();
    }
  }, [searchTerm, disciplinaFiltro, apenasFavoritos]);

  const loadPlanejamentos = async () => {
    try {
      setError(null);
      const params: { search?: string; disciplinaId?: number; favoritos?: boolean } = {};
      
      if (searchTerm.trim()) {
        params.search = searchTerm.trim();
      }
      
      if (disciplinaFiltro) {
        params.disciplinaId = Number(disciplinaFiltro);
      }
      
      if (apenasFavoritos) {
        params.favoritos = true;
      }

      const data = await planejamentoService.getAll(params);
      setPlanejamentos(data);
    } catch (err: any) {
      console.error('Erro ao carregar planejamentos:', err);
      setError('Erro ao carregar planejamentos. Tente novamente.');
    }
  };

  const handleDelete = async (id: number, titulo: string) => {
    if (!window.confirm(`Tem certeza que deseja excluir o planejamento "${titulo}"?`)) {
      return;
    }

    try {
      await planejamentoService.delete(id);
      await loadPlanejamentos();
    } catch (err: any) {
      console.error('Erro ao excluir planejamento:', err);
      showError('Erro ao excluir planejamento. Tente novamente.');
    }
  };

  const handleToggleFavorito = async (id: number) => {
    try {
      await planejamentoService.toggleFavorito(id);
      await loadPlanejamentos();
    } catch (err: any) {
      console.error('Erro ao atualizar favorito:', err);
      showError('Erro ao atualizar favorito. Tente novamente.');
    }
  };

  const handleFormSuccess = async () => {
    setShowForm(false);
    await loadPlanejamentos();
  };

  const handleFormCancel = () => {
    setShowForm(false);
  };

  return (
    <AuthenticatedLayout>
      <div className="planejamentos-content">
        {/* Cabeçalho da página */}
        <div className="page-header">
          <div className="page-title-section">
            <h1 className="page-title">Biblioteca de Planejamentos</h1>
            <p className="page-subtitle">
              Organize e reutilize seus planejamentos de aula
            </p>
          </div>
          <button 
            className="btn-novo-planejamento" 
            onClick={() => setShowForm(!showForm)}
          >
            <span className="btn-icon">+</span>
            Novo Planejamento
          </button>
        </div>

        {error && <ErrorMessage message={error} onDismiss={() => setError(null)} />}

        {/* Busca e Filtros */}
        <div className="filters-section">
          <div className="search-bar-container">
            <div className="search-icon">🔍</div>
            <input
              type="text"
              className="search-input"
              placeholder="Buscar por título, conteúdo ou objetivos..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
          
          <select
            className="filter-select"
            value={disciplinaFiltro}
            onChange={(e) => setDisciplinaFiltro(e.target.value ? Number(e.target.value) : '')}
          >
            <option value="">Todas as disciplinas</option>
            {disciplinas.map((disciplina) => (
              <option key={disciplina.id} value={disciplina.id}>
                {disciplina.nome}
              </option>
            ))}
          </select>
        </div>

        {/* Filtro de Favoritos */}
        <div className="favoritos-filter">
          <button
            className={`btn-favoritos ${apenasFavoritos ? 'active' : ''}`}
            onClick={() => setApenasFavoritos(!apenasFavoritos)}
          >
            <span className="star-icon">{apenasFavoritos ? '⭐' : '☆'}</span>
            Apenas Favoritos
          </button>
        </div>

        {loading && <Loading message="Carregando planejamentos..." />}

        {/* Formulário de cadastro */}
        {showForm && (
          <PlanejamentoForm 
            disciplinas={disciplinas}
            onSuccess={handleFormSuccess}
            onCancel={handleFormCancel}
          />
        )}

        {/* Grid de planejamentos */}
        {!loading && !error && (
          <div className="planejamentos-grid">
            {planejamentos.length > 0 ? (
              planejamentos.map((planejamento) => (
                <PlanejamentoCard
                  key={planejamento.id}
                  planejamento={planejamento}
                  onViewDetails={() => setDetalhesId(planejamento.id)}
                  onToggleFavorito={() => handleToggleFavorito(planejamento.id)}
                  onDelete={() => handleDelete(planejamento.id, planejamento.titulo)}
                />
              ))
            ) : (
              <div className="empty-state">
                <p>Nenhum planejamento encontrado.</p>
                {!searchTerm && !disciplinaFiltro && !apenasFavoritos && (
                  <button className="btn-novo-planejamento" onClick={() => setShowForm(true)}>
                    <span className="btn-icon">+</span>
                    Criar Primeiro Planejamento
                  </button>
                )}
              </div>
            )}
          </div>
        )}

        {/* Modal de detalhes */}
        {detalhesId !== null && (
          <PlanejamentoDetalhesModal
            planejamentoId={detalhesId}
            onClose={() => setDetalhesId(null)}
            onToggleFavorito={() => {
              handleToggleFavorito(detalhesId);
            }}
          />
        )}
      </div>
    </AuthenticatedLayout>
  );
};

