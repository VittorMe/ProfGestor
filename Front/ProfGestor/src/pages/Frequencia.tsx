import { useEffect, useState } from 'react';
import { AuthenticatedLayout } from '../components/Layout';
import { Loading } from '../components/UI/Loading';
import { ErrorMessage } from '../components/UI/ErrorMessage';
import { turmaService, type Turma } from '../services/turmaService';
import { frequenciaService, type StatusFrequencia, type FrequenciaAluno } from '../services/frequenciaService';
import { aulaService } from '../services/aulaService';
import { alunoService } from '../services/alunoService';
import './Frequencia.css';

interface AlunoFrequencia {
  alunoId: number;
  nome: string;
  matricula: string;
  status: StatusFrequencia | null;
}

export const Frequencia = () => {
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [turmaSelecionada, setTurmaSelecionada] = useState<number | ''>('');
  const [dataAula, setDataAula] = useState<string>('');
  const [alunos, setAlunos] = useState<AlunoFrequencia[]>([]);
  const [anotacao, setAnotacao] = useState<string>('');
  const [loading, setLoading] = useState(false);
  const [loadingAlunos, setLoadingAlunos] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchTurmas = async () => {
      try {
        setLoading(true);
        const data = await turmaService.getAll();
        setTurmas(data);
      } catch (err: any) {
        console.error('Erro ao carregar turmas:', err);
        setError('Erro ao carregar turmas. Tente novamente mais tarde.');
      } finally {
        setLoading(false);
      }
    };

    fetchTurmas();
  }, []);

  useEffect(() => {
    const fetchAlunos = async () => {
      if (!turmaSelecionada || !dataAula) {
        setAlunos([]);
        return;
      }

      try {
        setLoadingAlunos(true);
        setError(null);

        // Buscar alunos da turma
        const alunosData = await alunoService.getByTurmaId(Number(turmaSelecionada));
        const alunosList: AlunoFrequencia[] = alunosData.map(aluno => ({
          alunoId: aluno.id,
          nome: aluno.nome,
          matricula: aluno.matricula,
          status: null,
        }));

        // Buscar aula existente para esta data
        const aula = await aulaService.getByTurmaIdAndData(Number(turmaSelecionada), dataAula);
        
        if (aula) {
          // Buscar frequências da aula
          const frequencias = await frequenciaService.getByAulaId(aula.id);
          
          // Mapear alunos com suas frequências
          const alunosComFrequencia = alunosList.map(aluno => {
            const frequencia = frequencias.find(f => f.alunoId === aluno.alunoId);
            return {
              ...aluno,
              status: frequencia?.status || null,
            };
          });
          
          setAlunos(alunosComFrequencia);
          setAnotacao(aula.anotacaoTexto || '');
        } else {
          setAlunos(alunosList);
          setAnotacao('');
        }
      } catch (err: any) {
        console.error('Erro ao carregar dados:', err);
        setError('Erro ao carregar dados. Tente novamente.');
      } finally {
        setLoadingAlunos(false);
      }
    };

    fetchAlunos();
  }, [turmaSelecionada, dataAula]);

  const handleTurmaChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setTurmaSelecionada(e.target.value ? Number(e.target.value) : '');
    setDataAula('');
    setAlunos([]);
    setAnotacao('');
  };

  const handleAlunoStatusChange = (alunoId: number, status: StatusFrequencia) => {
    setAlunos(prev => prev.map(aluno => 
      aluno.alunoId === alunoId 
        ? { ...aluno, status }
        : aluno
    ));
  };

  const handleMarcarTodosPresentes = () => {
    setAlunos(prev => prev.map(aluno => ({
      ...aluno,
      status: 'PRESENTE' as StatusFrequencia,
    })));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!turmaSelecionada || !dataAula) {
      setError('Selecione a turma e a data da aula');
      return;
    }

    if (alunos.length === 0) {
      setError('Nenhum aluno encontrado para esta turma');
      return;
    }

    // Verificar se todos os alunos têm status definido
    const alunosSemStatus = alunos.filter(a => !a.status);
    if (alunosSemStatus.length > 0) {
      setError('Todos os alunos devem ter um status de frequência definido');
      return;
    }

    try {
      setLoading(true);

      const frequencias: FrequenciaAluno[] = alunos.map(aluno => ({
        alunoId: aluno.alunoId,
        status: aluno.status!,
      }));

      const periodo = '08:00 - 09:00'; // Default, pode ser ajustado

      await frequenciaService.registrarFrequencia({
        turmaId: Number(turmaSelecionada),
        dataAula,
        periodo,
        frequencias,
        anotacaoTexto: anotacao.trim() || undefined,
      });

      // Recarregar dados
      const aula = await aulaService.getByTurmaIdAndData(Number(turmaSelecionada), dataAula);
      if (aula) {
        const frequenciasData = await frequenciaService.getByAulaId(aula.id);
        setAlunos(prev => prev.map(aluno => {
          const frequencia = frequenciasData.find(f => f.alunoId === aluno.alunoId);
          return {
            ...aluno,
            status: frequencia?.status || null,
          };
        }));
        setAnotacao(aula.anotacaoTexto || '');
      }

      alert('Frequência registrada com sucesso!');
    } catch (err: any) {
      console.error('Erro ao registrar frequência:', err);
      setError(err.response?.data?.error || err.response?.data?.message || 'Erro ao registrar frequência. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthenticatedLayout>
      <div className="frequencia-content">
        <div className="frequencia-header">
          <div>
            <h1 className="page-title">Registro de Frequência</h1>
            <p className="page-subtitle">
              Registre a presença dos alunos por aula
            </p>
          </div>
        </div>

        {error && <ErrorMessage message={error} onDismiss={() => setError(null)} />}
        {loading && <Loading message="Carregando..." />}

        <form onSubmit={handleSubmit} className="frequencia-form">
          <div className="frequencia-main-grid">
            {/* Coluna Esquerda */}
            <div className="frequencia-left-column">
              {/* Seleção de Turma e Data */}
              <div className="frequencia-card">
                <h2 className="card-title">Selecionar Turma e Data</h2>
                <div className="form-row">
                  <div className="form-group">
                    <label htmlFor="turma" className="form-label">
                      Turma <span className="required">*</span>
                    </label>
                    <select
                      id="turma"
                      value={turmaSelecionada}
                      onChange={handleTurmaChange}
                      className="form-select"
                      required
                      disabled={loading}
                    >
                      <option value="">Selecione a turma</option>
                      {turmas.map(turma => (
                        <option key={turma.id} value={turma.id}>
                          {turma.nome} - {turma.disciplinaNome}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="form-group">
                    <label htmlFor="dataAula" className="form-label">
                      Data da Aula <span className="required">*</span>
                    </label>
                    <input
                      type="date"
                      id="dataAula"
                      value={dataAula}
                      onChange={(e) => setDataAula(e.target.value)}
                      className="form-input"
                      required
                      disabled={loading || !turmaSelecionada}
                      max={new Date().toISOString().split('T')[0]}
                    />
                  </div>
                </div>
              </div>

              {/* Chamada */}
              {turmaSelecionada && dataAula && (
                <div className="frequencia-card">
                  <div className="chamada-header">
                    <h2 className="card-title">Chamada</h2>
                    <button
                      type="button"
                      className="btn-marcar-todos"
                      onClick={handleMarcarTodosPresentes}
                      disabled={loading || alunos.length === 0}
                    >
                      Marcar Todos Presentes
                    </button>
                  </div>

                  {loadingAlunos ? (
                    <Loading message="Carregando alunos..." />
                  ) : alunos.length > 0 ? (
                    <ul className="alunos-list">
                      {alunos.map((aluno) => (
                        <li key={aluno.alunoId} className="aluno-item">
                          <div className="aluno-info">
                            <span className="aluno-icon">👤</span>
                            <span className="aluno-nome">{aluno.nome}</span>
                          </div>
                          <div className="aluno-actions">
                            <button
                              type="button"
                              className={`btn-status ${aluno.status === 'PRESENTE' ? 'active presente' : ''}`}
                              onClick={() => handleAlunoStatusChange(aluno.alunoId, 'PRESENTE')}
                              disabled={loading}
                            >
                              Presente
                            </button>
                            <button
                              type="button"
                              className={`btn-status ${aluno.status === 'FALTA' ? 'active falta' : ''}`}
                              onClick={() => handleAlunoStatusChange(aluno.alunoId, 'FALTA')}
                              disabled={loading}
                            >
                              Falta
                            </button>
                            <button
                              type="button"
                              className={`btn-status ${aluno.status === 'FALTA_JUSTIFICADA' ? 'active falta-justificada' : ''}`}
                              onClick={() => handleAlunoStatusChange(aluno.alunoId, 'FALTA_JUSTIFICADA')}
                              disabled={loading}
                            >
                              Justificada
                            </button>
                          </div>
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p className="empty-message">Nenhum aluno encontrado para esta turma</p>
                  )}
                </div>
              )}
            </div>

            {/* Coluna Direita */}
            <div className="frequencia-right-column">
              {/* Anotações */}
              <div className="frequencia-card">
                <h2 className="card-title">Anotações da Aula (Opcional)</h2>
                <textarea
                  id="anotacao"
                  value={anotacao}
                  onChange={(e) => {
                    if (e.target.value.length <= 1000) {
                      setAnotacao(e.target.value);
                    }
                  }}
                  className="form-textarea"
                  placeholder="Digite observações sobre a aula, conteúdo ministrado ou eventos relevantes..."
                  rows={10}
                  disabled={loading}
                />
                <div className="char-counter">
                  {anotacao.length}/1000
                </div>
              </div>

              {/* Botão Salvar */}
              <button
                type="submit"
                className="btn-salvar-frequencia"
                disabled={loading || !turmaSelecionada || !dataAula || alunos.length === 0}
              >
                <span className="btn-icon">📄</span>
                Salvar Frequência
              </button>
            </div>
          </div>
        </form>
      </div>
    </AuthenticatedLayout>
  );
};

