import api from '../config/api';
import type { Aula } from './frequenciaService';

export interface AulaCreate {
  data: string; // YYYY-MM-DD
  periodo: string;
  turmaId: number;
}

export const aulaService = {
  async getByTurmaId(turmaId: number): Promise<Aula[]> {
    const response = await api.get<Aula[]>(`/aulas/turma/${turmaId}`);
    return response.data;
  },

  async getByTurmaIdAndData(turmaId: number, data: string): Promise<Aula | null> {
    try {
      const response = await api.get<Aula>(`/aulas/turma/${turmaId}/data/${data}`);
      return response.data;
    } catch (error: any) {
      if (error.response?.status === 404) {
        return null;
      }
      throw error;
    }
  },
};

