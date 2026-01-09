import { useState } from 'react';
import { planejamentoService, type PlanejamentoCreate } from '../../services/planejamentoService';
import type { Disciplina } from '../../services/disciplinaService';
import { Loading } from '../UI/Loading';
import { ErrorMessage } from '../UI/ErrorMessage';
import './PlanejamentoForm.css';

interface PlanejamentoFormProps {
  disciplinas: Disciplina[];
  onSuccess: () => void;
  onCancel: () => void;
}

export const PlanejamentoForm = ({ disciplinas, onSuccess, onCancel }: PlanejamentoFormProps) => {
  const [formData, setFormData] = useState<PlanejamentoCreate>({
    titulo: '',
    dataAula: new Date().toISOString().split('T')[0],
    objetivos: '',
    conteudo: '',
    metodologia: '',
    favorito: false,
    disciplinaId: disciplinas[0]?.id || 0,
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!formData.titulo.trim()) {
      setError('O título é obrigatório');
      return;
    }

    if (!formData.disciplinaId) {
      setError('Selecione uma disciplina');
      return;
    }

    try {
      setLoading(true);
      setError(null);
      await planejamentoService.create(formData);
      onSuccess();
    } catch (err: any) {
      console.error('Erro ao criar planejamento:', err);
      setError(err.response?.data?.error || 'Erro ao criar planejamento. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="planejamento-form-container">
      <div className="form-header">
        <h2>Novo Planejamento</h2>
        <button className="form-close" onClick={onCancel}>×</button>
      </div>

      {error && <ErrorMessage message={error} onDismiss={() => setError(null)} />}

      <form onSubmit={handleSubmit} className="planejamento-form">
        <div className="form-row">
          <div className="form-group">
            <label htmlFor="titulo">Título *</label>
            <input
              type="text"
              id="titulo"
              value={formData.titulo}
              onChange={(e) => setFormData({ ...formData, titulo: e.target.value })}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="disciplinaId">Disciplina *</label>
            <select
              id="disciplinaId"
              value={formData.disciplinaId}
              onChange={(e) => setFormData({ ...formData, disciplinaId: Number(e.target.value) })}
              required
            >
              <option value="">Selecione...</option>
              {disciplinas.map((disciplina) => (
                <option key={disciplina.id} value={disciplina.id}>
                  {disciplina.nome}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="form-row">
          <div className="form-group">
            <label htmlFor="dataAula">Data da Aula *</label>
            <input
              type="date"
              id="dataAula"
              value={formData.dataAula}
              onChange={(e) => setFormData({ ...formData, dataAula: e.target.value })}
              required
            />
          </div>

          <div className="form-group checkbox-group">
            <label>
              <input
                type="checkbox"
                checked={formData.favorito}
                onChange={(e) => setFormData({ ...formData, favorito: e.target.checked })}
              />
              <span>Marcar como favorito</span>
            </label>
          </div>
        </div>

        <div className="form-group">
          <label htmlFor="objetivos">Objetivos</label>
          <textarea
            id="objetivos"
            rows={3}
            value={formData.objetivos}
            onChange={(e) => setFormData({ ...formData, objetivos: e.target.value })}
            placeholder="Descreva os objetivos da aula..."
          />
        </div>

        <div className="form-group">
          <label htmlFor="conteudo">Conteúdo</label>
          <textarea
            id="conteudo"
            rows={5}
            value={formData.conteudo}
            onChange={(e) => setFormData({ ...formData, conteudo: e.target.value })}
            placeholder="Descreva o conteúdo a ser abordado..."
          />
        </div>

        <div className="form-group">
          <label htmlFor="metodologia">Metodologia</label>
          <textarea
            id="metodologia"
            rows={4}
            value={formData.metodologia}
            onChange={(e) => setFormData({ ...formData, metodologia: e.target.value })}
            placeholder="Descreva a metodologia a ser utilizada..."
          />
        </div>

        <div className="form-actions">
          <button type="button" className="btn-cancel" onClick={onCancel} disabled={loading}>
            Cancelar
          </button>
          <button type="submit" className="btn-submit" disabled={loading}>
            {loading ? 'Salvando...' : 'Salvar Planejamento'}
          </button>
        </div>
      </form>

      {loading && <Loading message="Salvando planejamento..." />}
    </div>
  );
};

