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
    // Verificar se há usuário salvo no localStorage
    const loadUser = async () => {
      try {
        // Verificar autenticação primeiro
        if (!authService.isAuthenticated()) {
          setIsLoading(false);
          return;
        }

        const savedUser = authService.getCurrentUser();
        if (savedUser) {
          setUser(savedUser);
        } else {
          // Se não conseguir carregar usuário, limpar autenticação
          await authService.logout();
        }
      } catch (error) {
        console.error('Erro ao carregar usuário:', error);
        // Em caso de erro, limpar dados e garantir logout
        try {
          await authService.logout();
        } catch (logoutError) {
          console.error('Erro ao fazer logout após erro:', logoutError);
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
      await authService.login(email, password);
      // Buscar o usuário atualizado do localStorage
      const user = authService.getCurrentUser();
      setUser(user);
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

