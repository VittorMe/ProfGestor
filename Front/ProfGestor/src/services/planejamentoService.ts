import api from '../config/api';

export interface MaterialDidatico {
  id: number;
  nomeArquivo: string;
  tipoMime: string;
  tamanhoMB: number;
  urlArquivo: string;
  dataUpload: string;
}

export interface Planejamento {
  id: number;
  titulo: string;
  dataAula: string; // YYYY-MM-DD
  objetivos?: string;
  conteudo?: string;
  metodologia?: string;
  favorito: boolean;
  criadoEm: string;
  disciplinaId: number;
  disciplinaNome: string;
  materiaisDidaticos?: MaterialDidatico[];
}

export interface PlanejamentoCreate {
  titulo: string;
  dataAula: string; // YYYY-MM-DD
  objetivos?: string;
  conteudo?: string;
  metodologia?: string;
  favorito: boolean;
  disciplinaId: number;
}

export interface PlanejamentoUpdate {
  titulo: string;
  dataAula: string; // YYYY-MM-DD
  objetivos?: string;
  conteudo?: string;
  metodologia?: string;
  favorito: boolean;
  disciplinaId: number;
}

export const planejamentoService = {
  async getAll(params?: { search?: string; disciplinaId?: number; favoritos?: boolean }): Promise<Planejamento[]> {
    const queryParams = new URLSearchParams();
    if (params?.search) queryParams.append('search', params.search);
    if (params?.disciplinaId) queryParams.append('disciplinaId', params.disciplinaId.toString());
    if (params?.favoritos) queryParams.append('favoritos', 'true');
    
    const queryString = queryParams.toString();
    const url = `/planejamentosaula${queryString ? `?${queryString}` : ''}`;
    const response = await api.get<Planejamento[]>(url);
    return response.data;
  },

  async getById(id: number): Promise<Planejamento> {
    const response = await api.get<Planejamento>(`/planejamentosaula/${id}`);
    return response.data;
  },

  async getByDisciplinaId(disciplinaId: number): Promise<Planejamento[]> {
    const response = await api.get<Planejamento[]>(`/planejamentosaula/disciplina/${disciplinaId}`);
    return response.data;
  },

  async getFavoritos(): Promise<Planejamento[]> {
    const response = await api.get<Planejamento[]>(`/planejamentosaula/favoritos`);
    return response.data;
  },

  async create(planejamento: PlanejamentoCreate): Promise<Planejamento> {
    const response = await api.post<Planejamento>('/planejamentosaula', planejamento);
    return response.data;
  },

  async update(id: number, planejamento: PlanejamentoUpdate): Promise<Planejamento> {
    const response = await api.put<Planejamento>(`/planejamentosaula/${id}`, planejamento);
    return response.data;
  },

  async toggleFavorito(id: number): Promise<Planejamento> {
    const response = await api.patch<Planejamento>(`/planejamentosaula/${id}/favorito`);
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await api.delete(`/planejamentosaula/${id}`);
  },
};

