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
    
    // Validar resposta
    if (!response.data.professor) {
      throw new Error('Resposta de login inválida');
    }

    // Verificar se cookie foi aceito após login
    // Aguardar um pouco para o cookie ser processado
    setTimeout(async () => {
      const { verifyCookieAfterLogin } = await import('../utils/cookieDetection');
      const verification = await verifyCookieAfterLogin();
      if (!verification.working) {
        console.error('⚠️ ERRO: Cookies não estão funcionando!', verification.reason);
        console.error('⚠️ A autenticação falhará nas próximas requisições.');
        // Disparar evento customizado para o CookieWarning detectar
        window.dispatchEvent(new CustomEvent('cookieVerificationFailed', { 
          detail: verification 
        }));
      }
    }, 500);

    // Token está em cookie HttpOnly - não salvar nada no localStorage
    // Retornar apenas os dados da resposta
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
      // Chamar endpoint de logout no backend para remover cookie HttpOnly
      await api.post('/auth/logout');
    } catch (error) {
      console.error('Erro ao fazer logout no servidor:', error);
    }
    // Não precisa limpar localStorage - não salvamos nada
  },

  clearAuthData(): void {
    // Não há nada para limpar - tudo está no cookie HttpOnly
    // Esta função é mantida para compatibilidade, mas não faz nada
  },

  getCurrentUser(): User | null {
    // Não buscar do localStorage - sempre buscar do servidor
    // Esta função é mantida para compatibilidade, mas sempre retorna null
    return null;
  },

  getToken(): string | null {
    // Token agora está em cookie HttpOnly e não é acessível via JavaScript
    // O navegador envia automaticamente com withCredentials: true
    // Retornar null aqui, pois não precisamos mais ler o token
    return null;
  },

  isValidJWT(_token: string): boolean {
    // Token está em cookie HttpOnly, não precisamos mais validar no frontend
    // O backend valida automaticamente
    return true;
  },

  isTokenExpired(_token: string): boolean {
    // Não podemos verificar expiração sem acessar o token
    // O servidor valida automaticamente via cookie HttpOnly
    return false;
  },

  isAuthenticated(): boolean {
    // Não podemos verificar sem fazer requisição ao servidor
    // Retornar false - o AuthContext vai validar via servidor
    return false;
  },

  // ÚNICO método que busca dados do servidor
  async getCurrentUserFromServer(): Promise<User | null> {
    try {
      // Usar validateStatus para não rejeitar 401 como erro
      // Isso evita que apareça no console do navegador
      const response = await api.get<{ id: string; nome: string; email: string }>('/auth/me', {
        validateStatus: (status) => {
          // Aceitar 200 como sucesso, 401 como "não autenticado" (não é erro)
          return status === 200 || status === 401;
        }
      });
      
      // Se retornou 401, usuário não está autenticado
      if (response.status === 401) {
        return null;
      }
      
      // Se retornou 200, criar objeto User
      const user: User = {
        id: parseInt(response.data.id),
        email: response.data.email,
        name: response.data.nome,
      };
      // Não salvar no localStorage - apenas retornar
      return user;
    } catch (error: any) {
      // Outros erros devem ser logados
      console.error('Erro ao buscar usuário do servidor:', error);
      return null;
    }
  },
};

