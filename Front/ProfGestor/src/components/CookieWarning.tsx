import { useEffect, useState } from 'react';
import { detectCookieSupport, verifyCookieAfterLogin, type CookieSupport } from '../utils/cookieDetection';
import { useAuth } from '../contexts/AuthContext';
import './CookieWarning.css';

interface CookieWarningProps {
  onDismiss?: () => void;
}

export const CookieWarning = ({ onDismiss }: CookieWarningProps) => {
  const [cookieSupport, setCookieSupport] = useState<CookieSupport | null>(null);
  const [showWarning, setShowWarning] = useState(false);
  const [cookieWorking, setCookieWorking] = useState<boolean | null>(null);
  const { isAuthenticated } = useAuth();

  useEffect(() => {
    // Verificar suporte a cookies
    const support = detectCookieSupport();
    setCookieSupport(support);

    // Verificar se já foi dispensado anteriormente (usando sessionStorage)
    const dismissed = sessionStorage.getItem('cookieWarningDismissed') === 'true';

    // Mostrar aviso se cookies não estão habilitados e não foi dispensado
    if (!support.enabled && !dismissed) {
      console.warn('⚠️ Cookies detectados como desabilitados');
      setShowWarning(true);
    } else if (support.enabled) {
      console.log('✅ Cookies detectados como habilitados');
    }
  }, []);

  // Verificar se cookies estão funcionando após autenticação
  useEffect(() => {
    const checkCookieAfterAuth = async () => {
      if (isAuthenticated) {
        console.log('🔍 Verificando se cookies estão funcionando após login...');
        // Se está autenticado, verificar se cookie está realmente funcionando
        const verification = await verifyCookieAfterLogin();
        setCookieWorking(verification.working);
        
        console.log('📊 Resultado da verificação:', verification);
        
        // Se cookie não está funcionando, mostrar aviso
        if (!verification.working) {
          console.error('❌ Cookies não estão funcionando!', verification.reason);
          const dismissed = sessionStorage.getItem('cookieWarningDismissed') === 'true';
          if (!dismissed) {
            setShowWarning(true);
            setCookieSupport({
              enabled: false,
              httpOnlySupported: false,
              message: verification.reason || 'Cookies não estão funcionando corretamente. O login pode ter funcionado, mas as próximas requisições falharão.'
            });
          }
        } else {
          console.log('✅ Cookies estão funcionando corretamente!');
        }
      }
    };

    // Aguardar um pouco após autenticação para verificar
    if (isAuthenticated) {
      const timer = setTimeout(checkCookieAfterAuth, 1500); // Aumentado para 1.5s
      return () => clearTimeout(timer);
    }
  }, [isAuthenticated]);

  // Ouvir evento de falha na verificação de cookie
  useEffect(() => {
    const handleCookieVerificationFailed = (event: CustomEvent) => {
      const dismissed = sessionStorage.getItem('cookieWarningDismissed') === 'true';
      if (!dismissed) {
        setShowWarning(true);
        setCookieSupport({
          enabled: false,
          httpOnlySupported: false,
          message: event.detail?.reason || 'Cookies não estão funcionando corretamente'
        });
      }
    };

    window.addEventListener('cookieVerificationFailed', handleCookieVerificationFailed as EventListener);
    
    return () => {
      window.removeEventListener('cookieVerificationFailed', handleCookieVerificationFailed as EventListener);
    };
  }, []);

  const handleDismiss = () => {
    setShowWarning(false);
    // Salvar que foi dispensado apenas para esta sessão
    sessionStorage.setItem('cookieWarningDismissed', 'true');
    onDismiss?.();
  };

  const handleEnableCookies = () => {
    // Abrir guia de ajuda para habilitar cookies
    const userAgent = navigator.userAgent.toLowerCase();
    let helpUrl = '';

    if (userAgent.includes('chrome')) {
      helpUrl = 'https://support.google.com/chrome/answer/95647';
    } else if (userAgent.includes('firefox')) {
      helpUrl = 'https://support.mozilla.org/pt-BR/kb/habilitar-e-desabilitar-cookies-sites-rastreiam';
    } else if (userAgent.includes('safari')) {
      helpUrl = 'https://support.apple.com/pt-br/guide/safari/sfri11471/mac';
    } else if (userAgent.includes('edge')) {
      helpUrl = 'https://support.microsoft.com/pt-br/microsoft-edge/excluir-cookies-no-microsoft-edge-63947406-40ac-c3b8-57b9-2a946a29ae09';
    } else {
      helpUrl = 'https://www.google.com/search?q=como+habilitar+cookies';
    }

    window.open(helpUrl, '_blank');
  };

  // Mostrar aviso se:
  // 1. showWarning está true E
  // 2. cookieSupport existe E
  // 3. cookies não estão habilitados OU não estão funcionando
  if (!showWarning || !cookieSupport || (cookieSupport.enabled && cookieWorking !== false)) {
    return null;
  }

  return (
    <div className="cookie-warning-overlay">
      <div className="cookie-warning-container">
        <div className="cookie-warning-icon">🍪</div>
        <h2 className="cookie-warning-title">Cookies Necessários</h2>
        <p className="cookie-warning-message">
          Esta aplicação requer cookies para funcionar corretamente. 
          Os cookies são usados para manter sua sessão de login segura.
        </p>
        {cookieSupport?.message && (
          <p className="cookie-warning-details" style={{ 
            backgroundColor: '#fff3cd', 
            padding: '10px', 
            borderRadius: '4px',
            margin: '10px 0',
            color: '#856404'
          }}>
            <strong>⚠️ Problema detectado:</strong> {cookieSupport.message}
          </p>
        )}
        <p className="cookie-warning-details">
          Por favor, habilite cookies no seu navegador para continuar usando o ProfGestor.
          {cookieWorking === false && (
            <><br /><br /><strong>Nota:</strong> Você pode ter conseguido fazer login, mas as próximas requisições falharão sem cookies habilitados.</>
          )}
        </p>
        <div className="cookie-warning-actions">
          <button 
            className="cookie-warning-button cookie-warning-button-primary"
            onClick={handleEnableCookies}
          >
            Como Habilitar Cookies
          </button>
          <button 
            className="cookie-warning-button cookie-warning-button-secondary"
            onClick={handleDismiss}
          >
            Entendi (Continuar Mesmo Assim)
          </button>
        </div>
        <p className="cookie-warning-note">
          <small>
            Nota: Sem cookies habilitados, você não conseguirá fazer login ou acessar recursos protegidos.
          </small>
        </p>
      </div>
    </div>
  );
};
