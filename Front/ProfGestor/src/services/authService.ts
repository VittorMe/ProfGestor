import api from '../config/api';
import type { LoginRequest, LoginResponse, RegisterRequest, User } from '../types/auth';

export const authService = {
  async login(email: string, password: string): Promise<LoginResponse> {
    // Validar inputs básicos
    if (!email || !password) {
      throw new Error('Email e senha são obrigatórios');
    }

    // Sanitizar email (remover espaços)
    const sanitizedEmail = email.trim().toLowerCase();
    
    // Converter para o formato esperado pelo back-end (.NET)
    const loginData: LoginRequest = { 
      email: sanitizedEmail, 
      senha: password // Back-end espera "senha" (não "password")
    };
    
    const response = await api.post<LoginResponse>('/auth/login', loginData);
    
    // Validar resposta antes de salvar
    if (!response.data.token || !response.data.professor) {
      throw new Error('Resposta de login inválida');
    }

    // Validar formato do token antes de salvar
    if (!this.isValidJWT(response.data.token)) {
      throw new Error('Token recebido é inválido');
    }
    
    // Salvar token no localStorage
    localStorage.setItem('token', response.data.token);
    
    // Converter ProfessorInfo para User (formato esperado pelo front-end)
    const user: User = {
      id: response.data.professor.id,
      email: response.data.professor.email,
      name: response.data.professor.nome,
    };
    
    localStorage.setItem('user', JSON.stringify(user));
    
    // Salvar data de expiração se fornecida
    if (response.data.expiraEm) {
      localStorage.setItem('tokenExpiresAt', response.data.expiraEm);
    } else {
      // Tentar extrair do token se não for fornecido
      try {
        const payload = JSON.parse(atob(response.data.token.split('.')[1]));
        if (payload.exp) {
          const expirationDate = new Date(payload.exp * 1000);
          localStorage.setItem('tokenExpiresAt', expirationDate.toISOString());
        }
      } catch (error) {
        console.warn('Não foi possível extrair data de expiração do token');
      }
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
      console.error('Erro ao fazer logout no servidor:', error);
      // Continuar com logout local mesmo se falhar no servidor
    } finally {
      // Limpar todos os dados de autenticação
      this.clearAuthData();
    }
  },

  clearAuthData(): void {
    // Limpar todos os dados de autenticação de forma segura
    try {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      localStorage.removeItem('tokenExpiresAt');
    } catch (error) {
      console.error('Erro ao limpar dados de autenticação:', error);
      // Tentar limpar individualmente em caso de erro
      try {
        localStorage.clear();
      } catch (clearError) {
        console.error('Erro ao limpar localStorage:', clearError);
      }
    }
  },

  getCurrentUser(): User | null {
    // Verificar se está autenticado antes de retornar usuário
    if (!this.isAuthenticated()) {
      return null;
    }

    const userStr = localStorage.getItem('user');
    if (!userStr) {
      return null;
    }

    try {
      const user = JSON.parse(userStr);
      
      // Validar estrutura do usuário
      if (!user || typeof user.id !== 'number' || !user.email || !user.name) {
        console.warn('Dados do usuário inválidos');
        this.clearAuthData();
        return null;
      }
      
      return user;
    } catch (error) {
      console.error('Erro ao parsear usuário do localStorage:', error);
      this.clearAuthData();
      return null;
    }
  },

  getToken(): string | null {
    const token = localStorage.getItem('token');
    if (!token) return null;
    
    // Validar formato JWT
    if (!this.isValidJWT(token)) {
      console.warn('Token inválido: formato incorreto');
      this.logout();
      return null;
    }
    
    // Verificar expiração do token
    if (this.isTokenExpired(token)) {
      console.warn('Token expirado');
      this.logout();
      return null;
    }
    
    return token;
  },

  isValidJWT(token: string): boolean {
    // JWT tem 3 partes separadas por ponto: header.payload.signature
    const parts = token.split('.');
    if (parts.length !== 3) {
      return false;
    }
    
    // Verificar se cada parte é base64 válido
    try {
      parts.forEach(part => {
        if (part.length === 0) throw new Error('Parte vazia');
        // Tentar decodificar para validar base64
        atob(part.replace(/-/g, '+').replace(/_/g, '/'));
      });
      return true;
    } catch {
      return false;
    }
  },

  isTokenExpired(token: string): boolean {
    try {
      // Decodificar payload do JWT (segunda parte)
      const payload = JSON.parse(atob(token.split('.')[1]));
      
      // Verificar expiração (exp está em segundos Unix timestamp)
      if (payload.exp) {
        const expirationTime = payload.exp * 1000; // Converter para milissegundos
        const now = Date.now();
        
        // Adicionar margem de segurança de 5 minutos antes da expiração real
        const safetyMargin = 5 * 60 * 1000; // 5 minutos em milissegundos
        
        if (now >= (expirationTime - safetyMargin)) {
          return true;
        }
      }
      
      // Se não tiver exp, verificar tokenExpiresAt do localStorage como fallback
      const expiresAt = localStorage.getItem('tokenExpiresAt');
      if (expiresAt) {
        const expirationDate = new Date(expiresAt);
        if (expirationDate < new Date()) {
          return true;
        }
      }
      
      return false;
    } catch (error) {
      console.error('Erro ao verificar expiração do token:', error);
      // Se não conseguir decodificar, considerar expirado por segurança
      return true;
    }
  },

  isAuthenticated(): boolean {
    const token = this.getToken();
    return token !== null;
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

