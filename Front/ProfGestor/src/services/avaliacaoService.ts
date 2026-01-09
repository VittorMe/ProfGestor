import api from '../config/api';

export const TipoAvaliacao = {
  PROVA: 'PROVA',
  TRABALHO: 'TRABALHO',
  ATIVIDADE: 'ATIVIDADE',
  OUTRO: 'OUTRO',
} as const;

export type TipoAvaliacao = typeof TipoAvaliacao[keyof typeof TipoAvaliacao];

export interface Avaliacao {
  id: number;
  titulo: string;
  tipo: TipoAvaliacao;
  tipoDisplay: string; // Para exibição formatada (Mista, Subjetiva, etc)
  dataAplicacao: string; // YYYY-MM-DD
  valorMaximo: number;
  disciplinaId: number;
  disciplinaNome: string;
  temGabarito: boolean;
  temNotasLancadas: boolean;
  totalQuestoes: number;
}

export interface QuestaoObjetivaCreate {
  numero: number;
  enunciado: string;
  valor: number;
  alternativaCorreta?: string; // 'A', 'B', 'C', 'D' ou 'E'
}

export interface AvaliacaoCreate {
  titulo: string;
  tipo: TipoAvaliacao;
  dataAplicacao: string; // YYYY-MM-DD
  valorMaximo: number;
  disciplinaId: number;
  isObjetiva: boolean;
  questoesObjetivas?: QuestaoObjetivaCreate[];
}

export interface AvaliacaoUpdate {
  titulo: string;
  tipo: TipoAvaliacao;
  dataAplicacao: string; // YYYY-MM-DD
  valorMaximo: number;
  disciplinaId: number;
}

export const avaliacaoService = {
  async getAll(disciplinaId?: number): Promise<Avaliacao[]> {
    const params = disciplinaId ? `?disciplinaId=${disciplinaId}` : '';
    const response = await api.get<Avaliacao[]>(`/avaliacoes${params}`);
    return response.data;
  },

  async getById(id: number): Promise<Avaliacao> {
    const response = await api.get<Avaliacao>(`/avaliacoes/${id}`);
    return response.data;
  },

  async create(avaliacao: AvaliacaoCreate): Promise<Avaliacao> {
    const response = await api.post<Avaliacao>('/avaliacoes', avaliacao);
    return response.data;
  },

  async update(id: number, avaliacao: AvaliacaoUpdate): Promise<Avaliacao> {
    const response = await api.put<Avaliacao>(`/avaliacoes/${id}`, avaliacao);
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await api.delete(`/avaliacoes/${id}`);
  },
};

