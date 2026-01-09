import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { cleanupOldStorage, preventAuthStorage } from './utils/storageCleanup'

// Limpar dados antigos do localStorage imediatamente
cleanupOldStorage();

// Em desenvolvimento, bloquear tentativas de salvar dados de autenticação
preventAuthStorage();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
