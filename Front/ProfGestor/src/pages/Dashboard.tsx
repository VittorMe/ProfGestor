import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import './Dashboard.css';

export const Dashboard = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
      await logout();
      navigate('/login');
    } catch (error) {
      console.error('Erro ao fazer logout:', error);
      // Mesmo com erro, redirecionar para login
      navigate('/login');
    }
  };

  const metricCards = [
    { title: 'Turmas Ativas', value: '8', icon: '👥', color: '#3b82f6' },
    { title: 'Total de Alunos', value: '240', icon: '👥', color: '#10b981' },
    { title: 'Planejamentos', value: '45', icon: '📖', color: '#8b5cf6' },
    { title: 'Avaliações', value: '18', icon: '📄', color: '#f59e0b' },
  ];

  const quickActions = [
    { label: 'Registrar Frequência', icon: '✓' },
    { label: 'Criar Planejamento', icon: '📖' },
    { label: 'Lançar Notas', icon: '📄' },
    { label: 'Ver Relatórios', icon: '📊' },
  ];

  const nextClasses = [
    { class: '1A - Matemática', time: '10:00 - 11:00', room: 'Sala 8' },
    { class: '2A - História', time: '12:00 - 13:00', room: 'Sala 5' },
    { class: '1B - Geografia', time: '13:00 - 14:00', room: 'Sala 7' },
    { class: '3B - Ciências', time: '14:00 - 15:00', room: 'Sala 9' },
  ];

  const recentActivities = [
    { action: 'Frequência registrada', class: '1A - Matemática', time: 'Hoje, 10:30' },
    { action: 'Notas lançadas', class: '2B - História', time: 'Hoje, 09:15' },
    { action: 'Planejamento criado', class: '3C - Geografia', time: 'Ontem, 14:20' },
  ];

  return (
    <div className="dashboard-container">
      {/* Header com navegação */}
      <header className="dashboard-header">
        <div className="header-left">
          <div className="logo">
            <span className="logo-icon">📚</span>
            <span className="logo-text">ProfGestor</span>
          </div>
        </div>
        <div className="header-right">
          <nav className="header-nav">
            <a href="#" className="nav-item active">Início</a>
            <a href="#" className="nav-item">Turmas</a>
            <a href="#" className="nav-item">Frequência</a>
            <a href="#" className="nav-item">Planejamentos</a>
            <a href="#" className="nav-item">Avaliações</a>
            <a href="#" className="nav-item">Relatórios</a>
          </nav>
          <div className="user-menu">
            {user && (
              <span className="user-name">{user.name}</span>
            )}
            <button onClick={handleLogout} className="logout-button">
              <span className="logout-icon">🚪</span>
              Sair
            </button>
          </div>
        </div>
      </header>

      {/* Conteúdo principal */}
      <main className="dashboard-main">
        {/* Seção de boas-vindas */}
        <section className="welcome-section">
          <h1 className="welcome-title">Bem-vindo ao ProfGestor</h1>
          <p className="welcome-subtitle">
            Gerencie suas atividades pedagógicas de forma simples e organizada
          </p>
        </section>

        {/* Cards de métricas */}
        <section className="metrics-section">
          {metricCards.map((card, index) => (
            <div key={index} className="metric-card">
              <div className="metric-icon" style={{ backgroundColor: `${card.color}20`, color: card.color }}>
                {card.icon}
              </div>
              <div className="metric-content">
                <div className="metric-value">{card.value}</div>
                <div className="metric-title">{card.title}</div>
              </div>
            </div>
          ))}
        </section>

        {/* Grid de conteúdo */}
        <section className="content-grid">
          {/* Ações Rápidas */}
          <div className="content-card">
            <h2 className="card-title">Ações Rápidas</h2>
            <ul className="quick-actions-list">
              {quickActions.map((action, index) => (
                <li key={index} className="quick-action-item">
                  <span className="action-icon">{action.icon}</span>
                  <span className="action-label">{action.label}</span>
                </li>
              ))}
            </ul>
          </div>

          {/* Próximas Aulas */}
          <div className="content-card">
            <h2 className="card-title">Próximas Aulas</h2>
            <ul className="classes-list">
              {nextClasses.map((item, index) => (
                <li key={index} className="class-item">
                  <div className="class-info">
                    <div className="class-name">{item.class}</div>
                    <div className="class-details">
                      <span>{item.time}</span>
                      <span className="separator">•</span>
                      <span>{item.room}</span>
                    </div>
                  </div>
                </li>
              ))}
            </ul>
          </div>

          {/* Atividades Recentes */}
          <div className="content-card">
            <h2 className="card-title">Atividades Recentes</h2>
            <ul className="activities-list">
              {recentActivities.map((activity, index) => (
                <li key={index} className="activity-item">
                  <div className="activity-content">
                    <div className="activity-action">{activity.action}</div>
                    <div className="activity-class">{activity.class}</div>
                  </div>
                  <div className="activity-time">{activity.time}</div>
                </li>
              ))}
            </ul>
          </div>
        </section>
      </main>

      {/* Footer */}
      <footer className="dashboard-footer">
        <p>ProfGestor - Sistema de Gestão Pedagógica © 2025</p>
      </footer>
    </div>
  );
};
