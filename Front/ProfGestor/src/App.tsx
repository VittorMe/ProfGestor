import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { Dashboard } from './pages/Dashboard';
import { Turmas } from './pages/Turmas';
import { Frequencia } from './pages/Frequencia';
import { Planejamentos } from './pages/Planejamentos';
import { Avaliacoes } from './pages/Avaliacoes';
import { LancarNotas } from './pages/LancarNotas';
import { DefinirGabarito } from './pages/DefinirGabarito';
import { Relatorios } from './pages/Relatorios';
import { Test } from './pages/Test';
import './App.css';

const RootRedirect = () => {
  const { isAuthenticated, isLoading } = useAuth();
  
  if (isLoading) {
    return (
      <div style={{ 
        display: 'flex', 
        justifyContent: 'center', 
        alignItems: 'center', 
        height: '100vh',
        width: '100%',
        backgroundColor: '#f5f5f5',
        color: '#333',
        fontSize: '18px'
      }}>
        <div>Carregando...</div>
      </div>
    );
  }
  
  // Redirecionar baseado no estado de autenticação
  if (isAuthenticated) {
    return <Navigate to="/dashboard" replace />;
  }
  
  return <Navigate to="/login" replace />;
};

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/test" element={<Test />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route
            path="/dashboard"
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/turmas"
            element={
              <ProtectedRoute>
                <Turmas />
              </ProtectedRoute>
            }
          />
          <Route
            path="/frequencia"
            element={
              <ProtectedRoute>
                <Frequencia />
              </ProtectedRoute>
            }
          />
          <Route
            path="/planejamentos"
            element={
              <ProtectedRoute>
                <Planejamentos />
              </ProtectedRoute>
            }
          />
          <Route
            path="/avaliacoes"
            element={
              <ProtectedRoute>
                <Avaliacoes />
              </ProtectedRoute>
            }
          />
          <Route
            path="/avaliacoes/:avaliacaoId/lancar-notas"
            element={
              <ProtectedRoute>
                <LancarNotas />
              </ProtectedRoute>
            }
          />
          <Route
            path="/avaliacoes/:avaliacaoId/definir-gabarito"
            element={
              <ProtectedRoute>
                <DefinirGabarito />
              </ProtectedRoute>
            }
          />
          <Route
            path="/relatorios"
            element={
              <ProtectedRoute>
                <Relatorios />
              </ProtectedRoute>
            }
          />
          <Route path="/" element={<RootRedirect />} />
        </Routes>
        <ToastContainer
          position="top-right"
          autoClose={3000}
          hideProgressBar={false}
          newestOnTop={false}
          closeOnClick
          rtl={false}
          pauseOnFocusLoss
          draggable
          pauseOnHover
          theme="light"
        />
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
