import { useState, useEffect } from 'react';
import { AuthenticatedLayout } from '../components/Layout';
import { turmaService, type Turma } from '../services/turmaService';
import { relatorioService, type RelatorioFrequencia, type RelatorioDesempenho } from '../services/relatorioService';
import { Loading } from '../components/UI/Loading';
import { ErrorMessage } from '../components/UI/ErrorMessage';
import { showSuccess } from '../utils/toast';
import { exportarRelatorioFrequenciaPDF, exportarRelatorioDesempenhoPDF } from '../utils/pdfExporter';
import './Relatorios.css';

type TipoRelatorio = 'frequencia' | 'desempenho';

export const Relatorios = () => {
  const [tipoRelatorio, setTipoRelatorio] = useState<TipoRelatorio>('frequencia');
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [turmaId, setTurmaId] = useState<number | ''>('');
  const [dataInicio, setDataInicio] = useState<string>('');
  const [dataFim, setDataFim] = useState<string>('');
  const [periodo, setPeriodo] = useState<string>('');
  const [relatorioFrequencia, setRelatorioFrequencia] = useState<RelatorioFrequencia | null>(null);
  const [relatorioDesempenho, setRelatorioDesempenho] = useState<RelatorioDesempenho | null>(null);
  const [loading, setLoading] = useState(false);
  const [loadingTurmas, setLoadingTurmas] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchTurmas = async () => {
      try {
        setLoadingTurmas(true);
        const data = await turmaService.getAll();
        setTurmas(data);
      } catch (err: any) {
        console.error('Erro ao carregar turmas:', err);
        setError('Erro ao carregar turmas. Tente novamente.');
      } finally {
        setLoadingTurmas(false);
      }
    };

    fetchTurmas();
  }, []);

  const handleGerarRelatorio = async () => {
    if (!turmaId) {
      setError('Selecione uma turma.');
      return;
    }

    if (tipoRelatorio === 'frequencia') {
      if (!dataInicio || !dataFim) {
        setError('Selecione as datas de início e fim.');
        return;
      }
    }

    setError(null);
    setLoading(true);

    try {
      if (tipoRelatorio === 'frequencia') {
        const relatorio = await relatorioService.gerarRelatorioFrequencia({
          turmaId: Number(turmaId),
          dataInicio,
          dataFim,
        });
        setRelatorioFrequencia(relatorio);
        setRelatorioDesempenho(null);
      } else {
        const relatorio = await relatorioService.gerarRelatorioDesempenho({
          turmaId: Number(turmaId),
          periodo: periodo || undefined,
        });
        setRelatorioDesempenho(relatorio);
        setRelatorioFrequencia(null);
      }
    } catch (err: any) {
      console.error('Erro ao gerar relatório:', err);
      setError(err.response?.data?.error || 'Erro ao gerar relatório. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  const handleTipoRelatorioChange = (tipo: TipoRelatorio) => {
    setTipoRelatorio(tipo);
    setRelatorioFrequencia(null);
    setRelatorioDesempenho(null);
    setError(null);
  };

  return (
    <AuthenticatedLayout>
      <div className="relatorios-page">
        <div className="page-header">
          <h1>Relatórios</h1>
          <p>Gere relatórios detalhados de frequência e desempenho</p>
        </div>

        <div className="tipo-relatorio-tabs">
          <button
            className={`tab-button ${tipoRelatorio === 'frequencia' ? 'active' : ''}`}
            onClick={() => handleTipoRelatorioChange('frequencia')}
          >
            Relatório de Frequência
          </button>
          <button
            className={`tab-button ${tipoRelatorio === 'desempenho' ? 'active' : ''}`}
            onClick={() => handleTipoRelatorioChange('desempenho')}
          >
            Relatório de Desempenho
          </button>
        </div>

        {error && <ErrorMessage message={error} onDismiss={() => setError(null)} />}

        <div className="filtros-card">
          <h3>Filtros do Relatório</h3>
          <div className="filtros-grid">
            <div className="form-group">
              <label htmlFor="turma">Turma *</label>
              <select
                id="turma"
                value={turmaId}
                onChange={(e) => setTurmaId(e.target.value ? Number(e.target.value) : '')}
                required
                disabled={loadingTurmas}
              >
                <option value="">Selecione a turma</option>
                {turmas.map(turma => (
                  <option key={turma.id} value={turma.id}>
                    {turma.nome}
                  </option>
                ))}
              </select>
            </div>

            {tipoRelatorio === 'frequencia' ? (
              <>
                <div className="form-group">
                  <label htmlFor="dataInicio">Data de Início *</label>
                  <input
                    type="date"
                    id="dataInicio"
                    value={dataInicio}
                    onChange={(e) => setDataInicio(e.target.value)}
                    required
                  />
                </div>
                <div className="form-group">
                  <label htmlFor="dataFim">Data de Fim *</label>
                  <input
                    type="date"
                    id="dataFim"
                    value={dataFim}
                    onChange={(e) => setDataFim(e.target.value)}
                    required
                  />
                </div>
              </>
            ) : (
              <div className="form-group">
                <label htmlFor="periodo">Período (Opcional)</label>
                <select
                  id="periodo"
                  value={periodo}
                  onChange={(e) => setPeriodo(e.target.value)}
                >
                  <option value="">Selecione o período</option>
                  <option value="1º Bimestre">1º Bimestre</option>
                  <option value="2º Bimestre">2º Bimestre</option>
                  <option value="3º Bimestre">3º Bimestre</option>
                  <option value="4º Bimestre">4º Bimestre</option>
                  <option value="1º Trimestre">1º Trimestre</option>
                  <option value="2º Trimestre">2º Trimestre</option>
                  <option value="3º Trimestre">3º Trimestre</option>
                </select>
              </div>
            )}
          </div>

          <button
            className="btn-gerar-relatorio"
            onClick={handleGerarRelatorio}
            disabled={loading || loadingTurmas}
          >
            {loading ? 'Gerando...' : 'Gerar Relatório'}
          </button>
        </div>

        {loading && <Loading />}

        {tipoRelatorio === 'frequencia' && relatorioFrequencia && (
          <div className="relatorio-card">
            <div className="relatorio-header-with-action">
              <div>
                <h3>Relatório de Frequência</h3>
                <p className="relatorio-periodo">
                  Período: {new Date(relatorioFrequencia.dataInicio).toLocaleDateString('pt-BR')} a{' '}
                  {new Date(relatorioFrequencia.dataFim).toLocaleDateString('pt-BR')}
                </p>
              </div>
              <button 
                className="btn-exportar-pdf" 
                onClick={() => {
                  exportarRelatorioFrequenciaPDF(relatorioFrequencia);
                  showSuccess('PDF exportado com sucesso!');
                }}
              >
                📥 Exportar PDF
              </button>
            </div>

            <div className="relatorio-tabela">
              <table>
                <thead>
                  <tr>
                    <th>Aluno</th>
                    <th>Presenças</th>
                    <th>Faltas</th>
                    <th>Justificadas</th>
                    <th>Total</th>
                    <th>Assiduidade</th>
                  </tr>
                </thead>
                <tbody>
                  {relatorioFrequencia.alunos.map(aluno => (
                    <tr key={aluno.alunoId}>
                      <td className="aluno-nome">{aluno.alunoNome}</td>
                      <td className="presenca">{aluno.presencas}</td>
                      <td className="falta">{aluno.faltas}</td>
                      <td className="justificada">{aluno.faltasJustificadas}</td>
                      <td className="total">{aluno.totalAulas}</td>
                      <td>
                        <div className="assiduidade-cell">
                          <span className="percentual">{aluno.percentualPresenca.toFixed(1)}%</span>
                          <div className="progress-bar">
                            <div
                              className="progress-fill"
                              style={{ width: `${aluno.percentualPresenca}%` }}
                            />
                          </div>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            <div className="resumo-turma">
              <h4>Resumo da Turma</h4>
              <div className="resumo-grid">
                <div className="resumo-card verde">
                  <span className="resumo-label">Média de Assiduidade</span>
                  <span className="resumo-value">{relatorioFrequencia.mediaPresenca.toFixed(1)}%</span>
                </div>
                <div className="resumo-card verde">
                  <span className="resumo-label">Total de Presenças</span>
                  <span className="resumo-value">{relatorioFrequencia.totalPresencas}</span>
                </div>
                <div className="resumo-card vermelho">
                  <span className="resumo-label">Total de Faltas</span>
                  <span className="resumo-value">{relatorioFrequencia.totalFaltas}</span>
                </div>
                <div className="resumo-card laranja">
                  <span className="resumo-label">Faltas Justificadas</span>
                  <span className="resumo-value">{relatorioFrequencia.totalFaltasJustificadas}</span>
                </div>
              </div>
            </div>
          </div>
        )}

        {tipoRelatorio === 'desempenho' && relatorioDesempenho && (
          <div className="relatorio-card">
            <div className="relatorio-header-with-action">
              <div>
                <h3>Relatório de Desempenho da Turma</h3>
                {relatorioDesempenho.periodo && (
                  <p className="relatorio-periodo">Período: {relatorioDesempenho.periodo}</p>
                )}
              </div>
              <button 
                className="btn-exportar-pdf" 
                onClick={() => {
                  exportarRelatorioDesempenhoPDF(relatorioDesempenho);
                  showSuccess('PDF exportado com sucesso!');
                }}
              >
                📥 Exportar PDF
              </button>
            </div>

            <div className="resumo-desempenho-grid">
              <div className="resumo-card azul">
                <span className="resumo-label">Média Geral</span>
                <span className="resumo-value">{relatorioDesempenho.mediaGeralTurma.toFixed(1)}</span>
              </div>
              <div className="resumo-card verde">
                <span className="resumo-label">Mediana</span>
                <span className="resumo-value">{relatorioDesempenho.medianaTurma.toFixed(1)}</span>
              </div>
              <div className="resumo-card roxo">
                <span className="resumo-label">Maior Nota</span>
                <span className="resumo-value">{relatorioDesempenho.maiorNota.toFixed(1)}</span>
              </div>
              <div className="resumo-card laranja">
                <span className="resumo-label">Menor Nota</span>
                <span className="resumo-value">{relatorioDesempenho.menorNota.toFixed(1)}</span>
              </div>
              <div className="resumo-card verde-claro">
                <span className="resumo-label">Acima da Média</span>
                <span className="resumo-value">{relatorioDesempenho.qtdAcimaMedia} alunos</span>
              </div>
              <div className="resumo-card vermelho-claro">
                <span className="resumo-label">Abaixo da Média</span>
                <span className="resumo-value">{relatorioDesempenho.qtdAbaixoMedia} alunos</span>
              </div>
            </div>

            <div className="graficos-section">
              <div className="grafico-card">
                <h4>Distribuição de Notas</h4>
                <div className="bar-chart">
                  {relatorioDesempenho.distribuicaoNotas.map((item, index) => {
                    const maxQuantidade = Math.max(...relatorioDesempenho.distribuicaoNotas.map(d => d.quantidade));
                    const altura = maxQuantidade > 0 ? (item.quantidade / maxQuantidade) * 100 : 0;
                    return (
                      <div key={index} className="bar-item">
                        <div className="bar-container">
                          <div className="bar" style={{ height: `${altura}%` }}>
                            <span className="bar-value">{item.quantidade}</span>
                          </div>
                        </div>
                        <span className="bar-label">{item.faixa}</span>
                      </div>
                    );
                  })}
                </div>
              </div>

              <div className="grafico-card">
                <h4>Classificação de Desempenho</h4>
                <div className="pie-chart-container">
                  <div className="pie-chart-wrapper">
                    <svg className="pie-chart" viewBox="0 0 200 200">
                      {(() => {
                        const cores = ['#ef4444', '#f97316', '#3b82f6', '#10b981'];
                        let currentAngle = -90;
                        return relatorioDesempenho.classificacaoDesempenho.map((item, index) => {
                          const startAngle = currentAngle;
                          const angle = (item.percentual / 100) * 360;
                          const endAngle = startAngle + angle;
                          currentAngle = endAngle;
                          
                          const startRad = (startAngle * Math.PI) / 180;
                          const endRad = (endAngle * Math.PI) / 180;
                          const x1 = 100 + 80 * Math.cos(startRad);
                          const y1 = 100 + 80 * Math.sin(startRad);
                          const x2 = 100 + 80 * Math.cos(endRad);
                          const y2 = 100 + 80 * Math.sin(endRad);
                          const largeArc = angle > 180 ? 1 : 0;
                          const path = `M 100 100 L ${x1} ${y1} A 80 80 0 ${largeArc} 1 ${x2} ${y2} Z`;
                          
                          return (
                            <path
                              key={index}
                              d={path}
                              fill={cores[index]}
                              stroke="white"
                              strokeWidth="2"
                            />
                          );
                        });
                      })()}
                    </svg>
                  </div>
                  <div className="pie-legend">
                    {relatorioDesempenho.classificacaoDesempenho.map((item, index) => {
                      const cores = ['#ef4444', '#f97316', '#3b82f6', '#10b981'];
                      return (
                        <div key={index} className="legend-item">
                          <div className="legend-color" style={{ backgroundColor: cores[index] }} />
                          <span>{item.categoria} ({item.faixa}): {item.quantidade} alunos</span>
                        </div>
                      );
                    })}
                  </div>
                </div>
              </div>
            </div>

            {(relatorioDesempenho.observacao || relatorioDesempenho.recomendacao) && (
              <div className="analise-section">
                <h4>Análise e Recomendações</h4>
                {relatorioDesempenho.observacao && (
                  <div className="observacao-box">
                    <strong>Observação:</strong> {relatorioDesempenho.observacao}
                  </div>
                )}
                {relatorioDesempenho.recomendacao && (
                  <div className="recomendacao-box">
                    <strong>Recomendação:</strong> {relatorioDesempenho.recomendacao}
                  </div>
                )}
              </div>
            )}
          </div>
        )}

        {!relatorioFrequencia && !relatorioDesempenho && !loading && (
          <div className="empty-state">
            <div className="empty-icon">📊</div>
            <p className="empty-title">Configure os filtros</p>
            <p className="empty-description">
              {tipoRelatorio === 'frequencia'
                ? 'Selecione a turma e o período para gerar o relatório de frequência'
                : 'Selecione a turma para gerar o relatório de desempenho'}
            </p>
          </div>
        )}
      </div>
    </AuthenticatedLayout>
  );
};
