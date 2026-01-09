import { useEffect, useState } from 'react';
import type { Avaliacao, AvaliacaoCreate, AvaliacaoUpdate, QuestaoObjetivaCreate } from '../../services/avaliacaoService';
import { avaliacaoService, TipoAvaliacao } from '../../services/avaliacaoService';
import type { Disciplina } from '../../services/disciplinaService';
import { ErrorMessage } from '../UI/ErrorMessage';
import './AvaliacaoForm.css';

interface AvaliacaoFormProps {
  onSuccess: () => void;
  onCancel: () => void;
  disciplinas: Disciplina[];
  avaliacaoToEdit?: Avaliacao | null;
}

export const AvaliacaoForm = ({
  onSuccess,
  onCancel,
  disciplinas,
  avaliacaoToEdit,
}: AvaliacaoFormProps) => {
  const [titulo, setTitulo] = useState('');
  const [dataAplicacao, setDataAplicacao] = useState('');
  const [valorMaximo, setValorMaximo] = useState(10.0);
  const [disciplinaId, setDisciplinaId] = useState<number | ''>('');
  const [isObjetiva, setIsObjetiva] = useState(false);
  const [questoes, setQuestoes] = useState<QuestaoObjetivaCreate[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (avaliacaoToEdit) {
      setTitulo(avaliacaoToEdit.titulo);
      setDataAplicacao(avaliacaoToEdit.dataAplicacao);
      setValorMaximo(avaliacaoToEdit.valorMaximo);
      setDisciplinaId(avaliacaoToEdit.disciplinaId);
      setIsObjetiva(avaliacaoToEdit.totalQuestoes > 0);
    } else {
      // Reset form for new creation
      setTitulo('');
      setDataAplicacao('');
      setValorMaximo(10.0);
      setDisciplinaId('');
      setIsObjetiva(false);
      setQuestoes([]);
    }
  }, [avaliacaoToEdit]);

  const handleAddQuestao = () => {
    const numero = questoes.length + 1;
    setQuestoes([...questoes, {
      numero,
      enunciado: '',
      valor: 1.0,
      alternativaCorreta: undefined,
    }]);
  };

  const handleRemoveQuestao = (index: number) => {
    const novasQuestoes = questoes.filter((_, i) => i !== index);
    // Renumerar questões
    const questoesRenumeradas = novasQuestoes.map((q, i) => ({ ...q, numero: i + 1 }));
    setQuestoes(questoesRenumeradas);
  };

  const handleQuestaoChange = (index: number, field: keyof QuestaoObjetivaCreate, value: string | number | undefined) => {
    const novasQuestoes = [...questoes];
    novasQuestoes[index] = { ...novasQuestoes[index], [field]: value };
    setQuestoes(novasQuestoes);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    if (!titulo || !dataAplicacao || !disciplinaId) {
      setError('Título, Data de Aplicação e Disciplina são campos obrigatórios.');
      setLoading(false);
      return;
    }

    if (valorMaximo <= 0) {
      setError('O valor máximo deve ser maior que zero.');
      setLoading(false);
      return;
    }

    // Validar questões se for objetiva
    if (isObjetiva) {
      if (questoes.length === 0) {
        setError('Avaliações objetivas devem ter pelo menos uma questão.');
        setLoading(false);
        return;
      }

      // Validar que todas as questões têm enunciado
      const questoesInvalidas = questoes.filter(q => !q.enunciado.trim());
      if (questoesInvalidas.length > 0) {
        setError('Todas as questões devem ter um enunciado.');
        setLoading(false);
        return;
      }

      // Validar valores das questões
      const questoesComValorInvalido = questoes.filter(q => q.valor <= 0);
      if (questoesComValorInvalido.length > 0) {
        setError('Todas as questões devem ter um valor maior que zero.');
        setLoading(false);
        return;
      }
    }

    // Sempre usar PROVA como tipo padrão
    const payload: AvaliacaoCreate | AvaliacaoUpdate = {
      titulo,
      tipo: TipoAvaliacao.PROVA as TipoAvaliacao,
      dataAplicacao,
      valorMaximo,
      disciplinaId: Number(disciplinaId),
      ...(avaliacaoToEdit ? {} : {
        isObjetiva,
        questoesObjetivas: isObjetiva ? questoes.map(q => ({
          ...q,
          alternativaCorreta: q.alternativaCorreta ? q.alternativaCorreta.toUpperCase() : undefined,
        })) : undefined,
      } as Partial<AvaliacaoCreate>),
    };

    try {
      if (avaliacaoToEdit) {
        await avaliacaoService.update(avaliacaoToEdit.id, payload as AvaliacaoUpdate);
      } else {
        await avaliacaoService.create(payload as AvaliacaoCreate);
      }
      onSuccess();
    } catch (err: any) {
      console.error('Erro ao salvar avaliação:', err);
      setError(err.response?.data?.error || 'Erro ao salvar avaliação. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="avaliacao-form-overlay">
      <div className="avaliacao-form-container">
        <h3>{avaliacaoToEdit ? 'Editar Avaliação' : 'Nova Avaliação'}</h3>
        {error && <ErrorMessage message={error} onDismiss={() => setError(null)} />}
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="titulo">Título *</label>
            <input
              type="text"
              id="titulo"
              value={titulo}
              onChange={(e) => setTitulo(e.target.value)}
              required
              placeholder="Ex: Prova Bimestral - 1º Bimestre"
            />
          </div>

          <div className="form-group">
            <label htmlFor="disciplina">Disciplina *</label>
            <select
              id="disciplina"
              value={disciplinaId}
              onChange={(e) => setDisciplinaId(e.target.value === '' ? '' : Number(e.target.value))}
              required
            >
              <option value="">Selecione uma disciplina</option>
              {disciplinas.map(disciplina => (
                <option key={disciplina.id} value={disciplina.id}>
                  {disciplina.nome}
                </option>
              ))}
            </select>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="dataAplicacao">Data de Aplicação *</label>
              <input
                type="date"
                id="dataAplicacao"
                value={dataAplicacao}
                onChange={(e) => setDataAplicacao(e.target.value)}
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="valorMaximo">Valor Máximo (pontos) *</label>
              <input
                type="number"
                id="valorMaximo"
                value={valorMaximo}
                onChange={(e) => setValorMaximo(Number(e.target.value))}
                min="0.1"
                step="0.1"
                required
              />
            </div>
          </div>

          {!avaliacaoToEdit && (
            <div className="form-group">
              <label htmlFor="tipoAvaliacao">Tipo de Avaliação *</label>
              <select
                id="tipoAvaliacao"
                value={isObjetiva ? 'objetiva' : 'subjetiva'}
                onChange={(e) => {
                  const novaIsObjetiva = e.target.value === 'objetiva';
                  setIsObjetiva(novaIsObjetiva);
                  if (!novaIsObjetiva) {
                    setQuestoes([]);
                  } else if (questoes.length === 0) {
                    handleAddQuestao();
                  }
                }}
                required
              >
                <option value="subjetiva">Subjetiva</option>
                <option value="objetiva">Objetiva</option>
              </select>
            </div>
          )}

          {!avaliacaoToEdit && isObjetiva && (
            <div className="questoes-section">
              <div className="questoes-header">
                <h4>Questões Objetivas</h4>
                <button
                  type="button"
                  className="btn-add-questao"
                  onClick={handleAddQuestao}
                >
                  + Adicionar Questão
                </button>
              </div>

              {questoes.map((questao, index) => (
                <div key={index} className="questao-card">
                  <div className="questao-header">
                    <h5>Questão {questao.numero}</h5>
                    {questoes.length > 1 && (
                      <button
                        type="button"
                        className="btn-remove-questao"
                        onClick={() => handleRemoveQuestao(index)}
                      >
                        Remover
                      </button>
                    )}
                  </div>

                  <div className="form-group">
                    <label htmlFor={`enunciado-${index}`}>Enunciado *</label>
                    <textarea
                      id={`enunciado-${index}`}
                      value={questao.enunciado}
                      onChange={(e) => handleQuestaoChange(index, 'enunciado', e.target.value)}
                      required
                      rows={3}
                      placeholder="Digite o enunciado da questão..."
                    />
                  </div>

                  <div className="form-row">
                    <div className="form-group">
                      <label htmlFor={`valor-${index}`}>Valor (pontos) *</label>
                      <input
                        type="number"
                        id={`valor-${index}`}
                        value={questao.valor}
                        onChange={(e) => handleQuestaoChange(index, 'valor', Number(e.target.value))}
                        min="0.1"
                        step="0.1"
                        required
                      />
                    </div>

                    <div className="form-group">
                      <label htmlFor={`alternativa-${index}`}>Alternativa Correta</label>
                      <select
                        id={`alternativa-${index}`}
                        value={questao.alternativaCorreta || ''}
                        onChange={(e) => handleQuestaoChange(index, 'alternativaCorreta', e.target.value === '' ? undefined : e.target.value)}
                      >
                        <option value="">Selecione...</option>
                        <option value="A">A</option>
                        <option value="B">B</option>
                        <option value="C">C</option>
                        <option value="D">D</option>
                        <option value="E">E</option>
                      </select>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}

          <div className="form-actions">
            <button type="submit" className="btn-primary" disabled={loading}>
              {loading ? 'Salvando...' : 'Salvar'}
            </button>
            <button type="button" className="btn-secondary" onClick={onCancel} disabled={loading}>
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

