/**
 * Utilitário para limpar dados antigos do localStorage
 * Remove qualquer dado de autenticação que possa ter sido salvo anteriormente
 */
export const cleanupOldStorage = (): void => {
  try {
    // Remover todos os dados de autenticação antigos
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('tokenExpiresAt');
    
    // Log para debug (pode remover em produção)
    if (localStorage.getItem('token') || localStorage.getItem('user') || localStorage.getItem('tokenExpiresAt')) {
      console.warn('Aviso: Ainda há dados de autenticação no localStorage após limpeza');
    }
  } catch (error) {
    console.error('Erro ao limpar localStorage:', error);
  }
};

/**
 * Intercepta tentativas de salvar dados de autenticação no localStorage
 * Em desenvolvimento, mostra avisos se algo tentar salvar
 */
export const preventAuthStorage = (): void => {
  if (import.meta.env.DEV) {
    const originalSetItem = Storage.prototype.setItem;
    Storage.prototype.setItem = function(key: string, value: string) {
      // Bloquear salvamento de dados de autenticação
      if (key === 'token' || key === 'user' || key === 'tokenExpiresAt') {
        console.error(`🚫 BLOQUEADO: Tentativa de salvar "${key}" no localStorage!`);
        console.error('Stack trace:', new Error().stack);
        // Não salvar - retornar sem fazer nada
        return;
      }
      // Permitir outros itens
      originalSetItem.call(this, key, value);
    };
  }
};
