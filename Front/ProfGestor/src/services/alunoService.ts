import api from '../config/api';

export interface Aluno {
  id: number;
  nome: string;
  matricula: string;
  turmaId: number;
  turmaNome: string;
}

export const alunoService = {
  async getAll(): Promise<Aluno[]> {
    const response = await api.get<Aluno[]>('/alunos');
    return response.data;
  },

  async getById(id: number): Promise<Aluno> {
    const response = await api.get<Aluno>(`/alunos/${id}`);
    return response.data;
  },

  async getByTurmaId(turmaId: number): Promise<Aluno[]> {
    const response = await api.get<Aluno[]>(`/alunos/turma/${turmaId}`);
    return response.data;
  },
};


