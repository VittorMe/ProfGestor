import api from '../config/api';

export interface QuestaoGabarito {
  id: number;
  numero: number;
  enunciado: string;
  valor: number;
  alternativaCorreta?: string | null;
  temGabarito: boolean;
}

export interface GabaritoResumo {
  avaliacaoId: number;
  avaliacaoTitulo: string;
  disciplinaNome: string;
  turmaNome: string;
  questoes: QuestaoGabarito[];
}

export interface DefinirGabaritoRequest {
  avaliacaoId: number;
  itens: Array<{
    questaoObjetivaId: number;
    alternativaCorreta?: string | null;
  }>;
}

export const gabaritoService = {
  async getGabaritoResumo(avaliacaoId: number): Promise<GabaritoResumo> {
    const response = await api.get<GabaritoResumo>(`/gabaritos/avaliacao/${avaliacaoId}`);
    return response.data;
  },

  async definirGabarito(request: DefinirGabaritoRequest): Promise<void> {
    await api.post('/gabaritos/definir', request);
  },
};

