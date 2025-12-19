import api from '../config/api';

export interface Disciplina {
  id: number;
  nome: string;
}

export const disciplinaService = {
  async getAll(): Promise<Disciplina[]> {
    const response = await api.get<Disciplina[]>('/disciplinas');
    return response.data;
  },

  async getById(id: number): Promise<Disciplina> {
    const response = await api.get<Disciplina>(`/disciplinas/${id}`);
    return response.data;
  },
};

