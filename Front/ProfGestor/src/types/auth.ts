export interface LoginRequest {
  email: string;
  senha: string;
}

export interface RegisterRequest {
  nome: string;
  email: string;
  senha: string;
  confirmarSenha: string;
}

// Resposta do back-end .NET (configurado para usar camelCase)
export interface LoginResponse {
  token: string;
  expiraEm: string; // DateTime serializado como ISO string
  professor: ProfessorInfo;
}

export interface ProfessorInfo {
  id: number;
  nome: string;
  email: string;
}

export interface User {
  id: number;
  email: string;
  name: string;
  roles?: string[];
}

export interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
}

