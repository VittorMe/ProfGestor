import type { Planejamento } from '../../services/planejamentoService';
import './PlanejamentoCard.css';

interface PlanejamentoCardProps {
  planejamento: Planejamento;
  onViewDetails: () => void;
  onToggleFavorito: () => void;
  onDelete: () => void;
}

export const PlanejamentoCard = ({ 
  planejamento, 
  onViewDetails, 
  onToggleFavorito,
  onDelete 
}: PlanejamentoCardProps) => {
  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  };

  const primeiroMaterial = planejamento.materiaisDidaticos?.[0];

  return (
    <div className="planejamento-card">
      <div className="planejamento-card-header">
        <div className="planejamento-title-section">
          <h3 className="planejamento-titulo">{planejamento.titulo}</h3>
          <span className="planejamento-disciplina">
            {planejamento.disciplinaNome}
          </span>
        </div>
        <button
          className={`favorito-btn ${planejamento.favorito ? 'favorito' : ''}`}
          onClick={(e) => {
            e.stopPropagation();
            onToggleFavorito();
          }}
          title={planejamento.favorito ? 'Remover dos favoritos' : 'Adicionar aos favoritos'}
        >
          {planejamento.favorito ? '⭐' : '☆'}
        </button>
      </div>

      <div className="planejamento-card-body">
        <div className="planejamento-info-item">
          <span className="info-label">Data:</span>
          <span className="info-value">{formatDate(planejamento.dataAula)}</span>
        </div>
        
        {planejamento.objetivos && (
          <div className="planejamento-info-item">
            <span className="info-label">Objetivos:</span>
            <span className="info-value objetivos-text">
              {planejamento.objetivos.length > 80 
                ? `${planejamento.objetivos.substring(0, 80)}...` 
                : planejamento.objetivos}
            </span>
          </div>
        )}

        {primeiroMaterial && (
          <div className="material-didatico">
            <span className="material-icon">📎</span>
            <span className="material-nome">{primeiroMaterial.nomeArquivo}</span>
          </div>
        )}
      </div>

      <div className="planejamento-card-actions">
        <button 
          className="btn-ver-detalhes"
          onClick={onViewDetails}
        >
          <span className="btn-icon">👁️</span>
          Ver Detalhes
        </button>
        <button 
          className="btn-delete"
          onClick={(e) => {
            e.stopPropagation();
            onDelete();
          }}
          title="Excluir planejamento"
        >
          🗑️
        </button>
      </div>
    </div>
  );
};

