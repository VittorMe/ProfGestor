import api from '../config/api';

export interface Turma {
  id: number;
  nome: string;
  anoLetivo: number;
  semestre: number;
  turno: string;
  qtdAlunos: number;
  professorId: number;
  professorNome: string;
  disciplinaId: number;
  disciplinaNome: string;
  totalAlunos: number;
}

export interface TurmaCreate {
  nome: string;
  anoLetivo: number;
  semestre: number;
  turno: string;
  qtdAlunos: number;
  professorId: number;
  disciplinaId: number;
}

export const turmaService = {
  async getAll(): Promise<Turma[]> {
    const response = await api.get<Turma[]>('/turmas');
    return response.data;
  },

  async getById(id: number): Promise<Turma> {
    const response = await api.get<Turma>(`/turmas/${id}`);
    return response.data;
  },

  async create(turma: TurmaCreate): Promise<Turma> {
    const response = await api.post<Turma>('/turmas', turma);
    return response.data;
  },

  async update(id: number, turma: Partial<TurmaCreate>): Promise<Turma> {
    const response = await api.put<Turma>(`/turmas/${id}`, turma);
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await api.delete(`/turmas/${id}`);
  },
};

