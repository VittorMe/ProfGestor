import api from '../config/api';

export type StatusFrequencia = 'PRESENTE' | 'FALTA' | 'FALTA_JUSTIFICADA';

export interface Frequencia {
  id: number;
  status: StatusFrequencia;
  alunoId: number;
  alunoNome: string;
  alunoMatricula: string;
  aulaId: number;
}

export interface FrequenciaAluno {
  alunoId: number;
  status: StatusFrequencia;
}

export interface RegistrarFrequencia {
  turmaId: number;
  dataAula: string; // YYYY-MM-DD
  periodo: string;
  frequencias: FrequenciaAluno[];
  anotacaoTexto?: string;
}

export interface Aula {
  id: number;
  data: string;
  periodo: string;
  turmaId: number;
  turmaNome: string;
  disciplinaNome: string;
  temFrequenciaRegistrada: boolean;
  anotacaoTexto?: string;
}

export const frequenciaService = {
  async getByAulaId(aulaId: number): Promise<Frequencia[]> {
    const response = await api.get<Frequencia[]>(`/frequencias/aula/${aulaId}`);
    return response.data;
  },

  async registrarFrequencia(dto: RegistrarFrequencia): Promise<Aula> {
    const response = await api.post<Aula>('/frequencias/registrar', dto);
    return response.data;
  },

  async update(id: number, status: StatusFrequencia): Promise<Frequencia> {
    const response = await api.put<Frequencia>(`/frequencias/${id}`, { status });
    return response.data;
  },
};


