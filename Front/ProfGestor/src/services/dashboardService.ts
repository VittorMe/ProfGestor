import api from '../config/api';

export interface DashboardData {
  turmasAtivas: number;
  totalAlunos: number;
  planejamentos: number;
  avaliacoes: number;
  proximasAulas: ProximaAula[];
  atividadesRecentes: AtividadeRecente[];
}

export interface ProximaAula {
  turmaNome: string;
  disciplinaNome: string;
  periodo: string;
  data: string;
  sala: string;
}

export interface AtividadeRecente {
  acao: string;
  turmaNome: string;
  disciplinaNome: string;
  dataHora: string;
}

export const dashboardService = {
  async getDashboardData(): Promise<DashboardData> {
    // O endpoint é GET /api/dashboard 
    // Controller: DashboardController com [Route("api/[controller]")] e método GetDashboard() com [HttpGet]
    // Com baseURL='/api' e LowercaseUrls=true, a URL final será: /api/dashboard
    try {
      const response = await api.get<DashboardData>('/dashboard');
      return response.data;
    } catch (error: any) {
      console.error('Erro ao buscar dados do dashboard:', error);
      console.error('URL chamada:', error.config?.url || '/dashboard');
      console.error('Base URL:', error.config?.baseURL || '/api');
      throw error;
    }
  },
};

