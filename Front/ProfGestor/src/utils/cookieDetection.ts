/**
 * Utilitário para detectar suporte a cookies no navegador
 */

export interface CookieSupport {
  enabled: boolean;
  httpOnlySupported: boolean;
  message?: string;
}

/**
 * Verifica se cookies básicos estão habilitados
 */
export const checkBasicCookies = (): boolean => {
  try {
    // Primeiro, verificar navigator.cookieEnabled (mais confiável)
    if (navigator.cookieEnabled === false) {
      console.warn('🔒 navigator.cookieEnabled retornou false - cookies bloqueados');
      return false;
    }
    
    // Tentar criar um cookie de teste
    const testKey = `cookieTest_${Date.now()}_${Math.random().toString(36).substring(7)}`;
    const testValue = 'test_value_123';
    
    // Tentar criar o cookie
    document.cookie = `${testKey}=${testValue}; path=/; max-age=60`;
    
    // Aguardar um momento para o navegador processar (alguns navegadores são assíncronos)
    // Nota: Isso é uma limitação - não podemos fazer await aqui, mas vamos verificar imediatamente
    
    // Verificar se o cookie foi criado
    const cookieExists = document.cookie.indexOf(`${testKey}=${testValue}`) !== -1;
    
    // Limpar cookie de teste
    document.cookie = `${testKey}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
    
    if (!cookieExists) {
      console.warn('🔒 Cookie de teste não foi criado - cookies podem estar bloqueados');
    }
    
    return cookieExists;
  } catch (error) {
    console.error('Erro ao verificar cookies:', error);
    // Fallback: verificar navigator.cookieEnabled
    const fallback = navigator.cookieEnabled !== false;
    if (!fallback) {
      console.warn('🔒 Fallback: navigator.cookieEnabled indica cookies bloqueados');
    }
    return fallback;
  }
};

/**
 * Verifica se cookies HttpOnly são suportados
 * Nota: Não podemos testar HttpOnly diretamente via JavaScript,
 * mas podemos inferir baseado no suporte a cookies básicos
 */
export const checkHttpOnlySupport = (): boolean => {
  // Se cookies básicos funcionam, assumimos que HttpOnly também funciona
  // A limitação real é se o navegador bloqueia cookies de terceiros
  return checkBasicCookies();
};

/**
 * Verifica se cookies foram aceitos após uma requisição
 * Faz uma requisição autenticada e verifica se retorna sucesso
 */
export const checkCookieAfterRequest = async (): Promise<boolean> => {
  try {
    // Fazer uma requisição que requer autenticação
    // Se o cookie não foi aceito, retornará 401
    const response = await fetch('/api/auth/me', {
      method: 'GET',
      credentials: 'include', // Importante: incluir cookies
      headers: {
        'Content-Type': 'application/json',
      },
    });
    
    // Se retornar 200, o cookie foi aceito e enviado
    // Se retornar 401, o cookie não foi aceito ou não foi enviado
    return response.status === 200;
  } catch (error) {
    console.error('Erro ao verificar cookie após requisição:', error);
    return false;
  }
};

/**
 * Verifica se cookies estão funcionando após login
 * Tenta fazer uma requisição autenticada
 */
export const verifyCookieAfterLogin = async (): Promise<{
  working: boolean;
  reason?: string;
}> => {
  try {
    // Aguardar um pouco para o cookie ser processado pelo navegador
    await new Promise(resolve => setTimeout(resolve, 500));
    
    const response = await fetch('/api/auth/me', {
      method: 'GET',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json',
      },
    });
    
    if (response.status === 200) {
      return { working: true };
    } else if (response.status === 401) {
      return { 
        working: false, 
        reason: 'Cookie não foi aceito ou não está sendo enviado nas requisições' 
      };
    } else {
      return { 
        working: false, 
        reason: `Erro inesperado: ${response.status}` 
      };
    }
  } catch (error: any) {
    return { 
      working: false, 
      reason: error.message || 'Erro ao verificar cookie' 
    };
  }
};

/**
 * Detecta suporte completo a cookies
 */
export const detectCookieSupport = (): CookieSupport => {
  const basicEnabled = checkBasicCookies();
  const httpOnlySupported = checkHttpOnlySupport();
  
  let message: string | undefined;
  
  if (!basicEnabled) {
    message = 'Cookies estão desabilitados no seu navegador. Por favor, habilite cookies para usar esta aplicação.';
  } else if (!httpOnlySupported) {
    message = 'Cookies HttpOnly podem não estar funcionando. Algumas funcionalidades podem estar limitadas.';
  }
  
  return {
    enabled: basicEnabled,
    httpOnlySupported: httpOnlySupported && basicEnabled,
    message
  };
};

/**
 * Verifica se o navegador está em modo privado/incógnito
 * (Alguns navegadores bloqueiam cookies neste modo)
 */
export const isPrivateBrowsing = (): boolean => {
  try {
    // Tentar usar localStorage como teste
    // Em modo privado, alguns navegadores bloqueiam
    const test = '__private_test__';
    localStorage.setItem(test, '1');
    localStorage.removeItem(test);
    return false;
  } catch {
    // Se falhar, pode ser modo privado
    return true;
  }
};
