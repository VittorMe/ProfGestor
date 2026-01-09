import { createContext, useState, useEffect, useContext } from 'react';
import type { ReactNode } from 'react';
import { authService } from '../services/authService';
import type { User, AuthContextType } from '../types/auth';

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider = ({ children }: AuthProviderProps) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Sempre validar com o servidor ao carregar
    const loadUser = async () => {
      try {
        // Tentar buscar usuário do servidor (cookie HttpOnly será enviado automaticamente)
        const serverUser = await authService.getCurrentUserFromServer();
        if (serverUser) {
          setUser(serverUser);
        } else {
          // Se não retornou usuário, não está autenticado
          setUser(null);
        }
      } catch (error: any) {
        // Se der 401, não está autenticado - comportamento normal
        if (error.response?.status === 401) {
          setUser(null);
          // Não logar 401 como erro - é comportamento esperado
        } else {
          // Outro erro - logar apenas se for erro de rede/servidor
          if (error.code !== 'ERR_NETWORK' && error.response?.status !== 401) {
            console.warn('Erro ao validar autenticação:', error.message);
          }
          setUser(null);
        }
      } finally {
        setIsLoading(false);
      }
    };

    loadUser();
  }, []);

  const login = async (email: string, password: string): Promise<void> => {
    try {
      setIsLoading(true);
      const response = await authService.login(email, password);
      
      // Após login bem-sucedido, buscar usuário do servidor
      const serverUser = await authService.getCurrentUserFromServer();
      if (serverUser) {
        setUser(serverUser);
      } else {
        // Se não conseguiu buscar, converter da resposta do login
        const user: User = {
          id: response.professor.id,
          email: response.professor.email,
          name: response.professor.nome,
        };
        setUser(user);
      }
    } catch (error: any) {
      throw new Error(
        error.response?.data?.message || 'Erro ao fazer login. Verifique suas credenciais.'
      );
    } finally {
      setIsLoading(false);
    }
  };

  const logout = async (): Promise<void> => {
    try {
      await authService.logout();
    } finally {
      setUser(null);
    }
  };

  const value: AuthContextType = {
    user,
    isAuthenticated: !!user,
    isLoading,
    login,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth deve ser usado dentro de um AuthProvider');
  }
  return context;
};

