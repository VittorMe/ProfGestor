import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { AuthenticatedLayout } from '../components/Layout';
import { Loading } from '../components/UI/Loading';
import { ErrorMessage } from '../components/UI/ErrorMessage';
import { gabaritoService, type GabaritoResumo, type QuestaoGabarito } from '../services/gabaritoService';
import { showSuccess } from '../utils/toast';
import './DefinirGabarito.css';

const ALTERNATIVAS = ['A', 'B', 'C', 'D', 'E'];

export const DefinirGabarito = () => {
  const { avaliacaoId } = useParams<{ avaliacaoId: string }>();
  const navigate = useNavigate();
  const [resumo, setResumo] = useState<GabaritoResumo | null>(null);
  const [questoes, setQuestoes] = useState<QuestaoGabarito[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      if (!avaliacaoId) {
        setError('ID da avaliação não fornecido');
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        setError(null);
        const data = await gabaritoService.getGabaritoResumo(Number(avaliacaoId));
        setResumo(data);
        setQuestoes(data.questoes.map(q => ({ ...q }))); // Cópia para edição
      } catch (err: any) {
        console.error('Erro ao carregar dados do gabarito:', err);
        setError(err.response?.data?.error || 'Erro ao carregar dados. Tente novamente.');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [avaliacaoId]);

  const handleAlternativaChange = (questaoId: number, alternativa: string) => {
    setQuestoes(prev => prev.map(questao => {
      if (questao.id === questaoId) {
        return {
          ...questao,
          alternativaCorreta: alternativa === '' ? null : alternativa,
          temGabarito: alternativa !== ''
        };
      }
      return questao;
    }));
  };

  const handleSalvar = async () => {
    if (!resumo) return;

    try {
      setSaving(true);
      setError(null);

      const itens = questoes.map(q => ({
        questaoObjetivaId: q.id,
        alternativaCorreta: q.alternativaCorreta || null
      }));

      await gabaritoService.definirGabarito({
        avaliacaoId: resumo.avaliacaoId,
        itens
      });

      // Recarregar dados
      const data = await gabaritoService.getGabaritoResumo(resumo.avaliacaoId);
      setResumo(data);
      setQuestoes(data.questoes.map(q => ({ ...q })));
      
      showSuccess('Gabarito salvo com sucesso!');
    } catch (err: any) {
      console.error('Erro ao salvar gabarito:', err);
      setError(err.response?.data?.error || 'Erro ao salvar gabarito. Tente novamente.');
    } finally {
      setSaving(false);
    }
  };


  if (loading) {
    return (
      <AuthenticatedLayout>
        <Loading message="Carregando dados..." />
      </AuthenticatedLayout>
    );
  }

  if (error && !resumo) {
    return (
      <AuthenticatedLayout>
        <ErrorMessage message={error} onDismiss={() => navigate('/avaliacoes')} />
      </AuthenticatedLayout>
    );
  }

  if (!resumo || questoes.length === 0) {
    return (
      <AuthenticatedLayout>
        <div className="definir-gabarito-content">
          <div className="definir-gabarito-header">
            <button className="btn-voltar" onClick={() => navigate('/avaliacoes')}>
              ← Voltar
            </button>
            <div className="header-text">
              <h1>Definir Gabarito</h1>
            </div>
          </div>
          <div className="empty-state">
            <p>Esta avaliação não possui questões objetivas cadastradas.</p>
            <button className="btn-voltar" onClick={() => navigate('/avaliacoes')}>
              Voltar para Avaliações
            </button>
          </div>
        </div>
      </AuthenticatedLayout>
    );
  }

  return (
    <AuthenticatedLayout>
      <div className="definir-gabarito-content">
        <div className="definir-gabarito-header">
          <button className="btn-voltar" onClick={() => navigate('/avaliacoes')}>
            ← Voltar
          </button>
          <div className="header-text">
            <h1>Definir Gabarito</h1>
            <p className="context-info">
              {resumo.avaliacaoTitulo} • {resumo.disciplinaNome} • {resumo.turmaNome}
            </p>
          </div>
        </div>

        {error && <ErrorMessage message={error} onDismiss={() => setError(null)} />}

        <div className="gabarito-section">
          <div className="gabarito-section-header">
            <h2>Respostas Corretas</h2>
            <button 
              className="btn-salvar-gabarito" 
              onClick={handleSalvar}
              disabled={saving}
            >
              <span className="btn-icon">💾</span>
              {saving ? 'Salvando...' : 'Salvar Gabarito'}
            </button>
          </div>

          <div className="questoes-grid">
            {questoes.map(questao => (
              <div key={questao.id} className="questao-card">
                <div className="questao-header">
                  <h3>Questão {questao.numero}</h3>
                </div>
                <select
                  className="alternativa-select"
                  value={questao.alternativaCorreta || ''}
                  onChange={(e) => handleAlternativaChange(questao.id, e.target.value)}
                >
                  <option value="">Selecione</option>
                  {ALTERNATIVAS.map(alt => (
                    <option key={alt} value={alt}>
                      {alt}
                    </option>
                  ))}
                </select>
              </div>
            ))}
          </div>
        </div>

        <div className="resumo-section">
          <h3>Resumo do Gabarito</h3>
          <div className="resumo-content">
            {questoes.map(questao => (
              <div key={questao.id} className="resumo-item">
                {questao.numero}: {questao.alternativaCorreta || '-'}
              </div>
            ))}
          </div>
        </div>
      </div>
    </AuthenticatedLayout>
  );
};

