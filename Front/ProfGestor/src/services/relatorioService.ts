import api from '../config/api';

export interface RelatorioFrequenciaRequest {
  turmaId: number;
  dataInicio: string; // YYYY-MM-DD
  dataFim: string; // YYYY-MM-DD
}

export interface AlunoFrequenciaRelatorio {
  alunoId: number;
  alunoNome: string;
  matricula: string;
  totalAulas: number;
  presencas: number;
  faltas: number;
  faltasJustificadas: number;
  percentualPresenca: number;
}

export interface RelatorioFrequencia {
  turmaId: number;
  turmaNome: string;
  disciplinaNome: string;
  dataInicio: string;
  dataFim: string;
  geradoEm: string;
  alunos: AlunoFrequenciaRelatorio[];
  totalAulas: number;
  mediaPresenca: number;
  totalPresencas: number;
  totalFaltas: number;
  totalFaltasJustificadas: number;
}

export interface RelatorioDesempenhoRequest {
  turmaId: number;
  periodo?: string; // Opcional: "1º Bimestre", "2º Bimestre", etc.
}

export interface AvaliacaoDesempenho {
  avaliacaoId: number;
  titulo: string;
  dataAplicacao: string;
  valorMaximo: number;
  nota?: number;
  percentual?: number;
}

export interface AlunoDesempenhoRelatorio {
  alunoId: number;
  alunoNome: string;
  matricula: string;
  totalAvaliacoes: number;
  mediaGeral: number;
  somaNotas: number;
  somaValorMaximo: number;
  avaliacoes: AvaliacaoDesempenho[];
}

export interface DistribuicaoNotas {
  faixa: string;
  quantidade: number;
}

export interface ClassificacaoDesempenho {
  categoria: string;
  faixa: string;
  quantidade: number;
  percentual: number;
}

export interface RelatorioDesempenho {
  turmaId: number;
  turmaNome: string;
  disciplinaNome: string;
  periodo?: string;
  geradoEm: string;
  alunos: AlunoDesempenhoRelatorio[];
  mediaGeralTurma: number;
  medianaTurma: number;
  maiorNota: number;
  menorNota: number;
  qtdAcimaMedia: number;
  qtdAbaixoMedia: number;
  distribuicaoNotas: DistribuicaoNotas[];
  classificacaoDesempenho: ClassificacaoDesempenho[];
  observacao?: string;
  recomendacao?: string;
}

export const relatorioService = {
  async gerarRelatorioFrequencia(request: RelatorioFrequenciaRequest): Promise<RelatorioFrequencia> {
    const response = await api.post<RelatorioFrequencia>('/relatorios/frequencia', request);
    return response.data;
  },

  async gerarRelatorioDesempenho(request: RelatorioDesempenhoRequest): Promise<RelatorioDesempenho> {
    const response = await api.post<RelatorioDesempenho>('/relatorios/desempenho', request);
    return response.data;
  },
};
