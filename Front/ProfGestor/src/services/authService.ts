import api from '../config/api';
import type { LoginRequest, LoginResponse, RegisterRequest, User } from '../types/auth';

export const authService = {
  async login(email: string, password: string): Promise<LoginResponse> {
    // Converter para o formato esperado pelo back-end (.NET)
    const loginData: LoginRequest = { 
      email, 
      senha: password // Back-end espera "senha" (não "password")
    };
    
    const response = await api.post<LoginResponse>('/auth/login', loginData);
    
    // Salvar token no localStorage
    if (response.data.token && response.data.professor) {
      localStorage.setItem('token', response.data.token);
      
      // Converter ProfessorInfo para User (formato esperado pelo front-end)
      const user: User = {
        id: response.data.professor.id,
        email: response.data.professor.email,
        name: response.data.professor.nome,
      };
      
      localStorage.setItem('user', JSON.stringify(user));
      localStorage.setItem('tokenExpiresAt', response.data.expiraEm);
    }
    
    return response.data;
  },

  async register(nome: string, email: string, senha: string, confirmarSenha: string): Promise<void> {
    const registerData: RegisterRequest = {
      nome,
      email,
      senha,
      confirmarSenha,
    };
    
    await api.post('/auth/register', registerData);
  },

  async logout(): Promise<void> {
    try {
      // Se o back-end tiver endpoint de logout, chamar aqui
      // await api.post('/auth/logout');
    } catch (error) {
      console.error('Erro ao fazer logout:', error);
    } finally {
      // Limpar dados locais mesmo se a requisição falhar
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      localStorage.removeItem('tokenExpiresAt');
    }
  },

  getCurrentUser(): User | null {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      try {
      return JSON.parse(userStr);
      } catch (error) {
        console.error('Erro ao parsear usuário do localStorage:', error);
        return null;
      }
    }
    return null;
  },

  getToken(): string | null {
    return localStorage.getItem('token');
  },

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;
    
    // Verificar se o token expirou (opcional)
    const expiresAt = localStorage.getItem('tokenExpiresAt');
    if (expiresAt) {
      const expirationDate = new Date(expiresAt);
      if (expirationDate < new Date()) {
        // Token expirado, limpar dados
        this.logout();
        return false;
      }
    }
    
    return true;
  },

  // Buscar usuário atual do servidor (opcional, útil para validar token)
  async getCurrentUserFromServer(): Promise<User | null> {
    try {
      const response = await api.get<{ id: string; nome: string; email: string }>('/auth/me');
      const user: User = {
        id: parseInt(response.data.id),
        email: response.data.email,
        name: response.data.nome,
      };
      localStorage.setItem('user', JSON.stringify(user));
      return user;
    } catch (error) {
      console.error('Erro ao buscar usuário do servidor:', error);
      return null;
    }
  },
};

