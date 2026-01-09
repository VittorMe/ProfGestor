export interface NavItem {
  path: string;
  label: string;
  icon: string;
}

export const NAV_ITEMS: NavItem[] = [
  { path: '/dashboard', label: 'Início', icon: '🏠' },
  { path: '/turmas', label: 'Turmas', icon: '👥' },
  { path: '/frequencia', label: 'Frequência', icon: '✓' },
  { path: '/planejamentos', label: 'Planejamentos', icon: '📖' },
  { path: '/avaliacoes', label: 'Avaliações', icon: '📄' },
  { path: '/relatorios', label: 'Relatórios', icon: '📊' },
];

