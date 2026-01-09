import { useState } from 'react';
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
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const handleLogout = () => {
    if (onLogout) {
      onLogout();
    }
  };

  const toggleMobileMenu = () => {
    setIsMobileMenuOpen(!isMobileMenuOpen);
  };

  const closeMobileMenu = () => {
    setIsMobileMenuOpen(false);
  };

  return (
    <>
      {isMobileMenuOpen && (
        <div className="mobile-menu-overlay" onClick={closeMobileMenu} />
      )}
      <header className="app-header">
        <div className="header-left">
          <div className="logo">
            <span className="logo-icon">📚</span>
            <span className="logo-text">ProfGestor</span>
          </div>
        </div>
        <div className="header-right">
          <button className="mobile-menu-toggle" onClick={toggleMobileMenu} aria-label="Toggle menu">
            <span className={`hamburger ${isMobileMenuOpen ? 'active' : ''}`}>
              <span></span>
              <span></span>
              <span></span>
            </span>
          </button>
          <nav className={`header-nav ${isMobileMenuOpen ? 'mobile-open' : ''}`}>
            {NAV_ITEMS.map((item, index) => {
              const isActive = location.pathname === item.path;
              const className = `nav-item ${isActive ? 'active' : ''}`;
              // Usar índice + label como chave única para evitar duplicatas
              const uniqueKey = `${item.path}-${item.label}-${index}`;

              if (item.path === '#') {
                return (
                  <a key={uniqueKey} href={item.path} className={className} onClick={closeMobileMenu}>
                    <span className="nav-icon">{item.icon}</span>
                    {item.label}
                  </a>
                );
              }

              return (
                <Link key={uniqueKey} to={item.path} className={className} onClick={closeMobileMenu}>
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
    </>
  );
};

