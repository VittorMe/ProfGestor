import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthenticatedLayout } from '../components/Layout/AuthenticatedLayout';
import { Loading } from '../components/UI/Loading';
import { ErrorMessage } from '../components/UI/ErrorMessage';
import { dashboardService, type DashboardData, type ProximaAula, type AtividadeRecente } from '../services/dashboardService';
import { formatDate, formatClassTime } from '../utils/dateFormatters';
import './Dashboard.css';

export const Dashboard = () => {
  const navigate = useNavigate();
  const [dashboardData, setDashboardData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchDashboardData = async () => {
      try {
        setLoading(true);
        const data = await dashboardService.getDashboardData();
        setDashboardData(data);
        setError(null);
      } catch (err: any) {
        console.error('Erro ao carregar dados do dashboard:', err);
        setError('Erro ao carregar dados do dashboard. Tente novamente mais tarde.');
      } finally {
        setLoading(false);
      }
    };

    fetchDashboardData();
  }, []);

  const metricCards = dashboardData ? [
    { title: 'Turmas Ativas', value: dashboardData.turmasAtivas.toString(), icon: '👥', color: '#3b82f6' },
    { title: 'Total de Alunos', value: dashboardData.totalAlunos.toString(), icon: '👥', color: '#10b981' },
    { title: 'Planejamentos', value: dashboardData.planejamentos.toString(), icon: '📖', color: '#8b5cf6' },
    { title: 'Avaliações', value: dashboardData.avaliacoes.toString(), icon: '📄', color: '#f59e0b' },
  ] : [
    { title: 'Turmas Ativas', value: '0', icon: '👥', color: '#3b82f6' },
    { title: 'Total de Alunos', value: '0', icon: '👥', color: '#10b981' },
    { title: 'Planejamentos', value: '0', icon: '📖', color: '#8b5cf6' },
    { title: 'Avaliações', value: '0', icon: '📄', color: '#f59e0b' },
  ];

  const handleQuickAction = (action: string) => {
    switch (action) {
      case 'Registrar Frequência':
        navigate('/frequencia');
        break;
      case 'Criar Planejamento':
        navigate('/planejamentos');
        break;
      case 'Lançar Notas':
        navigate('/avaliacoes');
        break;
      case 'Ver Relatórios':
        navigate('/relatorios');
        break;
      default:
        break;
    }
  };

  const quickActions = [
    { label: 'Registrar Frequência', icon: '✓', path: '/frequencia' },
    { label: 'Criar Planejamento', icon: '📖', path: '/planejamentos' },
    { label: 'Lançar Notas', icon: '📄', path: '/avaliacoes' },
    { label: 'Ver Relatórios', icon: '📊', path: '/relatorios' },
  ];

  const nextClasses: Array<{ class: string; time: string; room: string }> = dashboardData?.proximasAulas.map((aula: ProximaAula) => ({
    class: `${aula.turmaNome} - ${aula.disciplinaNome}`,
    time: formatClassTime(aula.periodo, aula.data),
    room: aula.sala,
  })) || [];

  const recentActivities: Array<{ action: string; class: string; time: string }> = dashboardData?.atividadesRecentes.map((atividade: AtividadeRecente) => ({
    action: atividade.acao,
    class: `${atividade.turmaNome} - ${atividade.disciplinaNome}`,
    time: formatDate(atividade.dataHora),
  })) || [];

  return (
    <AuthenticatedLayout>
      <div className="dashboard-content">
        {/* Seção de boas-vindas */}
        <section className="welcome-section">
          <h1 className="welcome-title">Bem-vindo ao ProfGestor</h1>
          <p className="welcome-subtitle">
            Gerencie suas atividades pedagógicas de forma simples e organizada
          </p>
        </section>

        {error && <ErrorMessage message={error} onDismiss={() => setError(null)} />}
        {loading && <Loading message="Carregando dados do dashboard..." />}

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
                <li 
                  key={index} 
                  className="quick-action-item"
                  onClick={() => handleQuickAction(action.label)}
                  style={{ cursor: 'pointer' }}
                >
                  <span className="action-icon">{action.icon}</span>
                  <span className="action-label">{action.label}</span>
                </li>
              ))}
            </ul>
          </div>

          {/* Próximas Aulas */}
          <div className="content-card">
            <h2 className="card-title">Próximas Aulas</h2>
            {nextClasses.length > 0 ? (
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
            ) : (
              <p style={{ padding: '1rem', color: '#6b7280', textAlign: 'center' }}>
                Nenhuma aula agendada
              </p>
            )}
          </div>

          {/* Atividades Recentes */}
          <div className="content-card">
            <h2 className="card-title">Atividades Recentes</h2>
            {recentActivities.length > 0 ? (
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
            ) : (
              <p style={{ padding: '1rem', color: '#6b7280', textAlign: 'center' }}>
                Nenhuma atividade recente
              </p>
            )}
          </div>
        </section>
      </div>
    </AuthenticatedLayout>
  );
};
