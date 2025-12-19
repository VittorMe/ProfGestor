export interface NavItem {
  path: string;
  label: string;
  icon: string;
}

export const NAV_ITEMS: NavItem[] = [
  { path: '/dashboard', label: 'Início', icon: '🏠' },
  { path: '/turmas', label: 'Turmas', icon: '👥' },
  { path: '/frequencia', label: 'Frequência', icon: '✓' },
  { path: '#', label: 'Planejamentos', icon: '📖' },
  { path: '#', label: 'Avaliações', icon: '📄' },
  { path: '#', label: 'Relatórios', icon: '📊' },
];

