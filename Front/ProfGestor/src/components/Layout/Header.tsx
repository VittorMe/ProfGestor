import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { NAV_ITEMS } from '../../constants/navigation';
import './Header.css';

interface HeaderProps {
  onLogout?: () => void;
}

export const Header = ({ onLogout }: HeaderProps) => {
  const { user } = useAuth();
  const location = useLocation();

  const handleLogout = () => {
    if (onLogout) {
      onLogout();
    }
  };

  return (
    <header className="app-header">
      <div className="header-left">
        <div className="logo">
          <span className="logo-icon">📚</span>
          <span className="logo-text">ProfGestor</span>
        </div>
      </div>
      <div className="header-right">
        <nav className="header-nav">
          {NAV_ITEMS.map((item) => {
            const isActive = location.pathname === item.path;
            const className = `nav-item ${isActive ? 'active' : ''}`;

            if (item.path === '#') {
              return (
                <a key={item.path} href={item.path} className={className}>
                  <span className="nav-icon">{item.icon}</span>
                  {item.label}
                </a>
              );
            }

            return (
              <Link key={item.path} to={item.path} className={className}>
                <span className="nav-icon">{item.icon}</span>
                {item.label}
              </Link>
            );
          })}
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
  );
};

