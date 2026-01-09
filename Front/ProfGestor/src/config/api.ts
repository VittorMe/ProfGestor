import axios from 'axios';

// URL base da API .NET
// Usa /api relativo ao domínio (funciona com NGINX em produção)
// Para desenvolvimento local, configure VITE_API_BASE_URL no .env se necessário
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '/api';

// Criar instância do axios
export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor para adicionar token de autenticação
api.interceptors.request.use(
  (config) => {
    // Importar authService de forma dinâmica para evitar dependência circular
    const token = localStorage.getItem('token');
    
    if (token) {
      // Validar token antes de usar
      try {
        // Verificar formato básico JWT (3 partes)
        const parts = token.split('.');
        if (parts.length === 3) {
          // Verificar expiração básica
          try {
            const payload = JSON.parse(atob(parts[1]));
            if (payload.exp) {
              const expirationTime = payload.exp * 1000;
              const now = Date.now();
              // Se expirado, não adicionar token
              if (now >= expirationTime) {
                console.warn('Token expirado na requisição');
                localStorage.removeItem('token');
                localStorage.removeItem('user');
                localStorage.removeItem('tokenExpiresAt');
                // Redirecionar para login se não estiver já na página de login
                if (!window.location.pathname.includes('/login')) {
                  window.location.href = '/login';
                }
                return Promise.reject(new Error('Token expirado'));
              }
            }
          } catch {
            // Se não conseguir decodificar, continuar (deixar servidor validar)
          }
        }
        
        config.headers.Authorization = `Bearer ${token}`;
      } catch (error) {
        console.error('Erro ao validar token na requisição:', error);
      }
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor para tratar erros de autenticação
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Não logar erros 404 - são esperados em alguns casos (ex: buscar aula que não existe)
    if (error.response?.status === 404) {
      // Retornar erro silenciosamente para ser tratado pelo código que chamou
      return Promise.reject(error);
    }

    if (error.response?.status === 401) {
      // Token inválido ou expirado
      console.warn('Erro 401: Token inválido ou expirado');
      
      // Limpar dados de autenticação
      try {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        localStorage.removeItem('tokenExpiresAt');
      } catch (clearError) {
        console.error('Erro ao limpar dados de autenticação:', clearError);
      }
      
      // Redirecionar para login apenas se não estiver já na página de login
      if (!window.location.pathname.includes('/login') && 
          !window.location.pathname.includes('/register')) {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

export default api;

