import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        // Proxy para o backend .NET
        // Tenta porta 44324 primeiro (IIS Express), depois 7224 (dotnet run)
        target: 'https://localhost:44324',
        changeOrigin: true,
        secure: false, // Ignorar certificado SSL auto-assinado em desenvolvimento
        ws: true, // Habilitar WebSocket se necessário
        configure: (proxy, _options) => {
          proxy.on('error', (err, _req, _res) => {
            console.error('Erro no proxy:', err);
            // Se falhar na porta 44324, pode tentar 7224
          });
        },
      }
    }
  }
})
