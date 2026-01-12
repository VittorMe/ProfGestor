import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { AuthenticatedLayout } from '../components/Layout';
import { Loading } from '../components/UI/Loading';
import { ErrorMessage } from '../components/UI/ErrorMessage';
import { notaService, type LancamentoNotasResumo, type AlunoNota } from '../services/notaService';
import { showSuccess } from '../utils/toast';
import './LancarNotas.css';

export const LancarNotas = () => {
  const { avaliacaoId } = useParams<{ avaliacaoId: string }>();
  const navigate = useNavigate();
  const [resumo, setResumo] = useState<LancamentoNotasResumo | null>(null);
  const [alunosNotas, setAlunosNotas] = useState<AlunoNota[]>([]);
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
        const data = await notaService.getLancamentoNotas(Number(avaliacaoId));
        setResumo(data);
        setAlunosNotas(data.alunos.map(a => ({ ...a }))); // Cópia para edição
      } catch (err: any) {
        console.error('Erro ao carregar dados de lançamento de notas:', err);
        setError(err.response?.data?.error || 'Erro ao carregar dados. Tente novamente.');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [avaliacaoId]);

  const handleNotaChange = (alunoId: number, value: string) => {
    const numValue = value === '' ? null : Number(value);
    
    setAlunosNotas(prev => prev.map(aluno => {
      if (aluno.alunoId === alunoId) {
        return {
          ...aluno,
          nota: numValue,
          temNota: numValue !== null && numValue !== undefined && numValue > 0
        };
      }
      return aluno;
    }));
  };

  const handleSalvar = async () => {
    if (!resumo) return;

    // Validar notas
    const notasInvalidas = alunosNotas.filter(a => 
      a.nota !== null && a.nota !== undefined && (a.nota < 0 || a.nota > resumo.valorMaximo)
    );

    if (notasInvalidas.length > 0) {
      setError(`Algumas notas estão fora do intervalo permitido (0 a ${resumo.valorMaximo}).`);
      return;
    }

    try {
      setSaving(true);
      setError(null);

      const notasParaEnviar = alunosNotas
        .filter(a => a.nota !== null && a.nota !== undefined && a.nota >= 0)
        .map(a => ({
          alunoId: Number(a.alunoId), // Garantir que é number
          valor: Number(a.nota!) // Garantir que é number
        }));

      // Validar se há notas para enviar
      if (notasParaEnviar.length === 0) {
        setError('Nenhuma nota válida para salvar.');
        setSaving(false);
        return;
      }

      // Validar IDs e valores
      const notasInvalidas = notasParaEnviar.filter(n => 
        !n.alunoId || n.alunoId <= 0 || isNaN(n.valor) || n.valor < 0
      );

      if (notasInvalidas.length > 0) {
        setError('Algumas notas contêm dados inválidos.');
        setSaving(false);
        return;
      }

      const requestData = {
        avaliacaoId: Number(resumo.avaliacaoId), // Garantir que é number
        notas: notasParaEnviar
      };

      console.log('Enviando dados:', JSON.stringify(requestData, null, 2));

      await notaService.lancarNotas(requestData);

      // Recarregar dados
      const data = await notaService.getLancamentoNotas(resumo.avaliacaoId);
      setResumo(data);
      setAlunosNotas(data.alunos.map(a => ({ ...a })));
      
      showSuccess('Notas salvas com sucesso!');
    } catch (err: any) {
      console.error('Erro ao salvar notas:', err);
      setError(err.response?.data?.error || 'Erro ao salvar notas. Tente novamente.');
    } finally {
      setSaving(false);
    }
  };

  const calcularMedia = () => {
    const notasValidas = alunosNotas
      .filter(a => a.temNota && a.nota !== null && a.nota !== undefined)
      .map(a => a.nota!);
    
    if (notasValidas.length === 0) return 0;
    return notasValidas.reduce((sum, nota) => sum + nota, 0) / notasValidas.length;
  };

  const contarNotasLancadas = () => {
    return alunosNotas.filter(a => a.temNota && a.nota !== null && a.nota !== undefined && a.nota > 0).length;
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

  if (!resumo) {
    return null;
  }

  const mediaAtual = calcularMedia();
  const notasLancadasAtual = contarNotasLancadas();

  return (
    <AuthenticatedLayout>
      <div className="lancar-notas-content">
        <div className="lancar-notas-header">
          <button className="btn-voltar" onClick={() => navigate('/avaliacoes')}>
            ← Voltar
          </button>
          <div className="header-text">
            <h1>Lançamento de Notas</h1>
            <p className="context-info">
              {resumo.avaliacaoTitulo} • {resumo.disciplinaNome} • {resumo.turmaNome}
            </p>
          </div>
        </div>

        <div className="resumo-cards">
          <div className="resumo-card">
            <div className="resumo-card-label">Valor Máximo</div>
            <div className="resumo-card-value">{resumo.valorMaximo.toFixed(1)}</div>
          </div>
          <div className="resumo-card">
            <div className="resumo-card-label">Média da Turma</div>
            <div className="resumo-card-value">{mediaAtual.toFixed(2)}</div>
          </div>
          <div className="resumo-card">
            <div className="resumo-card-label">Notas Lançadas</div>
            <div className="resumo-card-value">{notasLancadasAtual}/{resumo.totalAlunos}</div>
          </div>
        </div>

        {error && <ErrorMessage message={error} onDismiss={() => setError(null)} />}

        <div className="notas-section">
          <div className="notas-section-header">
            <h2>Notas dos Alunos</h2>
            <button 
              className="btn-salvar-notas" 
              onClick={handleSalvar}
              disabled={saving}
            >
              <span className="btn-icon">💾</span>
              {saving ? 'Salvando...' : 'Salvar Notas'}
            </button>
          </div>

          <div className="notas-table-container">
            <table className="notas-table">
              <thead>
                <tr>
                  <th>Aluno</th>
                  <th>Nota</th>
                  <th>Valor Máximo</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {alunosNotas.map(aluno => (
                  <tr key={aluno.alunoId}>
                    <td className="aluno-cell">
                      <div className="aluno-info">
                        <div className="aluno-iniciais">{aluno.iniciais}</div>
                        <div className="aluno-nome">{aluno.alunoNome}</div>
                      </div>
                    </td>
                    <td className="nota-cell">
                      <input
                        type="number"
                        className="nota-input"
                        value={aluno.nota ?? ''}
                        onChange={(e) => handleNotaChange(aluno.alunoId, e.target.value)}
                        min="0"
                        max={resumo.valorMaximo}
                        step="0.1"
                        placeholder="0.0"
                      />
                    </td>
                    <td className="valor-maximo-cell">
                      {resumo.valorMaximo.toFixed(1)}
                    </td>
                    <td className="status-cell">
                      <span className={`status-badge ${aluno.temNota && aluno.nota && aluno.nota > 0 ? 'lancada' : 'pendente'}`}>
                        {aluno.temNota && aluno.nota && aluno.nota > 0 ? 'Lançada' : 'Pendente'}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </AuthenticatedLayout>
  );
};

