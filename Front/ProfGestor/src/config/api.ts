import axios from 'axios';

// URL base da API .NET
// Em desenvolvimento: usa /api para passar pelo proxy do Vite (mantém cookies)
// Em produção: usa /api relativo ao domínio (funciona com NGINX)
// IMPORTANTE: Não definir VITE_API_BASE_URL em desenvolvimento para usar o proxy
const isDevelopment = import.meta.env.DEV;
const API_BASE_URL = isDevelopment 
  ? '/api' // Sempre usar proxy em desenvolvimento para manter cookies
  : (import.meta.env.VITE_API_BASE_URL || '/api'); // Em produção, pode usar URL completa

// Criar instância do axios
export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true, // IMPORTANTE: Enviar cookies automaticamente (incluindo HttpOnly)
});

// Interceptor para requisições
// O token agora está em cookie HttpOnly e é enviado automaticamente pelo navegador
// Não precisamos mais adicionar manualmente no header Authorization
api.interceptors.request.use(
  (config) => {
    // O token está em cookie HttpOnly e será enviado automaticamente
    // Não precisamos fazer nada aqui
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
      // Não logar como erro - pode ser comportamento esperado (usuário não autenticado)
      // Não limpar localStorage - não salvamos nada lá
      // Apenas rejeitar o erro silenciosamente para o AuthContext gerenciar
      return Promise.reject(error);
    }
    
    // Outros erros devem ser logados normalmente
    return Promise.reject(error);
  }
);

export default api;

