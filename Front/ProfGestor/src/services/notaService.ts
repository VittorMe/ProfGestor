import api from '../config/api';

export interface AlunoNota {
  alunoId: number;
  alunoNome: string;
  alunoMatricula: string;
  iniciais: string;
  nota?: number | null;
  temNota: boolean;
  dataLancamento?: string | null;
}

export interface LancamentoNotasResumo {
  avaliacaoId: number;
  avaliacaoTitulo: string;
  disciplinaNome: string;
  turmaNome: string;
  valorMaximo: number;
  mediaTurma: number;
  notasLancadas: number;
  totalAlunos: number;
  alunos: AlunoNota[];
}

export interface LancarNotasRequest {
  avaliacaoId: number;
  notas: Array<{
    alunoId: number;
    valor: number;
  }>;
}

export const notaService = {
  async getLancamentoNotas(avaliacaoId: number): Promise<LancamentoNotasResumo> {
    const response = await api.get<LancamentoNotasResumo>(`/notasAvaliacao/avaliacao/${avaliacaoId}`);
    return response.data;
  },

  async lancarNotas(request: LancarNotasRequest): Promise<void> {
    await api.post('/notasAvaliacao/lancar', request);
  },
};

