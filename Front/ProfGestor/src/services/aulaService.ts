import api from '../config/api';
import type { Aula } from './frequenciaService';

// Re-exportar Aula para uso em outros componentes
export type { Aula };

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
      // Usar validateStatus para não tratar 404 como erro
      const response = await api.get<Aula>(`/aulas/turma/${turmaId}/data/${data}`, {
        validateStatus: (status) => status === 200 || status === 404
      });
      
      // Se retornou 404, não há aula para aquela data (comportamento esperado)
      if (response.status === 404) {
        return null;
      }
      
      return response.data;
    } catch (error: any) {
      // Se ainda assim ocorrer um erro, tratar
      if (error.response?.status === 404) {
        return null;
      }
      console.error('Erro ao buscar aula:', error);
      throw error;
    }
  },
};


