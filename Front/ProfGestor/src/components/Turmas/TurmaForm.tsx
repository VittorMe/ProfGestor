import { useState, useEffect } from 'react';
import { useAuth } from '../../contexts/AuthContext';
import { turmaService, type TurmaCreate } from '../../services/turmaService';
import { disciplinaService } from '../../services/disciplinaService';
import './TurmaForm.css';

interface TurmaFormProps {
  onSuccess: () => void;
  onCancel: () => void;
}

export const TurmaForm = ({ onSuccess, onCancel }: TurmaFormProps) => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [disciplinas, setDisciplinas] = useState<{ id: number; nome: string }[]>([]);
  
  const [formData, setFormData] = useState<TurmaCreate>({
    nome: '',
    anoLetivo: new Date().getFullYear(),
    semestre: 1,
    turno: '',
    qtdAlunos: 0,
    professorId: user?.id || 0,
    disciplinaId: 0,
  });

  useEffect(() => {
    const fetchDisciplinas = async () => {
      try {
        const data = await disciplinaService.getAll();
        setDisciplinas(data);
      } catch (err) {
        console.error('Erro ao carregar disciplinas:', err);
      }
    };
    fetchDisciplinas();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'anoLetivo' || name === 'semestre' || name === 'qtdAlunos' || name === 'disciplinaId' || name === 'professorId'
        ? parseInt(value) || 0
        : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    // Validação
    if (!formData.nome.trim()) {
      setError('Nome da turma é obrigatório');
      return;
    }
    if (!formData.turno) {
      setError('Turno é obrigatório');
      return;
    }
    if (!formData.disciplinaId) {
      setError('Disciplina é obrigatória');
      return;
    }
    if (!formData.semestre) {
      setError('Semestre é obrigatório');
      return;
    }

    try {
      setLoading(true);
      await turmaService.create(formData);
      onSuccess();
    } catch (err: any) {
      console.error('Erro ao criar turma:', err);
      setError(err.response?.data?.error || err.response?.data?.message || 'Erro ao criar turma. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="turma-form-container">
      <div className="turma-form-card">
        <h2 className="turma-form-title">Cadastrar Nova Turma</h2>
        
        {error && (
          <div className="turma-form-error">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="turma-form">
          <div className="turma-form-row">
            <div className="turma-form-column">
              <div className="turma-form-group">
                <label htmlFor="nome" className="turma-form-label">
                  Nome da Turma <span className="required">*</span>
                </label>
                <input
                  type="text"
                  id="nome"
                  name="nome"
                  value={formData.nome}
                  onChange={handleChange}
                  placeholder="Ex: 1A, 2B, 3C"
                  className="turma-form-input"
                  required
                />
              </div>

              <div className="turma-form-group">
                <label htmlFor="anoLetivo" className="turma-form-label">
                  Ano
                </label>
                <input
                  type="number"
                  id="anoLetivo"
                  name="anoLetivo"
                  value={formData.anoLetivo}
                  onChange={handleChange}
                  className="turma-form-input"
                />
              </div>

              <div className="turma-form-group">
                <label htmlFor="turno" className="turma-form-label">
                  Turno <span className="required">*</span>
                </label>
                <select
                  id="turno"
                  name="turno"
                  value={formData.turno}
                  onChange={handleChange}
                  className="turma-form-select"
                  required
                >
                  <option value="">Selecione o turno</option>
                  <option value="Matutino">Matutino</option>
                  <option value="Vespertino">Vespertino</option>
                  <option value="Noturno">Noturno</option>
                </select>
              </div>
            </div>

            <div className="turma-form-column">
              <div className="turma-form-group">
                <label htmlFor="disciplinaId" className="turma-form-label">
                  Disciplina <span className="required">*</span>
                </label>
                <select
                  id="disciplinaId"
                  name="disciplinaId"
                  value={formData.disciplinaId}
                  onChange={handleChange}
                  className="turma-form-select"
                  required
                >
                  <option value="">Selecione a disciplina</option>
                  {disciplinas.map((disciplina) => (
                    <option key={disciplina.id} value={disciplina.id}>
                      {disciplina.nome}
                    </option>
                  ))}
                </select>
              </div>

              <div className="turma-form-group">
                <label htmlFor="semestre" className="turma-form-label">
                  Semestre <span className="required">*</span>
                </label>
                <select
                  id="semestre"
                  name="semestre"
                  value={formData.semestre}
                  onChange={handleChange}
                  className="turma-form-select"
                  required
                >
                  <option value="">Selecione o semestre</option>
                  <option value="1">1º Semestre</option>
                  <option value="2">2º Semestre</option>
                </select>
              </div>
            </div>
          </div>

          <div className="turma-form-actions">
            <button
              type="submit"
              className="btn-cadastrar"
              disabled={loading}
            >
              {loading ? 'Cadastrando...' : 'Cadastrar'}
            </button>
            <button
              type="button"
              className="btn-cancelar"
              onClick={onCancel}
              disabled={loading}
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

