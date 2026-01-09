/**
 * Utilitário para manipulação de cookies
 * Nota: Cookies HttpOnly não podem ser lidos/modificados via JavaScript
 * Este utilitário é útil apenas para cookies não-HttpOnly
 */
export const cookieUtils = {
  /**
   * Obtém o valor de um cookie
   * @param name Nome do cookie
   * @returns Valor do cookie ou null se não existir
   */
  get(name: string): string | null {
    if (typeof document === 'undefined') return null;
    
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) {
      return parts.pop()?.split(';').shift() || null;
    }
    return null;
  },

  /**
   * Define um cookie
   * @param name Nome do cookie
   * @param value Valor do cookie
   * @param days Dias até expirar (padrão: 7)
   */
  set(name: string, value: string, days: number = 7): void {
    if (typeof document === 'undefined') return;
    
    const expires = new Date();
    expires.setTime(expires.getTime() + days * 24 * 60 * 60 * 1000);
    document.cookie = `${name}=${value};expires=${expires.toUTCString()};path=/;SameSite=Strict`;
  },

  /**
   * Remove um cookie
   * @param name Nome do cookie
   */
  remove(name: string): void {
    if (typeof document === 'undefined') return;
    
    document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;`;
  },

  /**
   * Verifica se um cookie existe
   * @param name Nome do cookie
   * @returns true se o cookie existe
   */
  exists(name: string): boolean {
    return this.get(name) !== null;
  }
};
