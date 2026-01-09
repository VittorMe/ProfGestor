import { useNavigate } from 'react-router-dom';
import type { Avaliacao } from '../../services/avaliacaoService';
import { formatDateOnly } from '../../utils/dateFormatters';
import './AvaliacaoCard.css';

interface AvaliacaoCardProps {
  avaliacao: Avaliacao;
  onLancarNotas?: (avaliacao: Avaliacao) => void;
  onDefinirGabarito?: (avaliacao: Avaliacao) => void;
  onEdit?: (avaliacao: Avaliacao) => void;
  onDelete?: (id: number, titulo: string) => void;
}

export const AvaliacaoCard = ({
  avaliacao,
}: AvaliacaoCardProps) => {
  const navigate = useNavigate();
  const formattedDate = formatDateOnly(avaliacao.dataAplicacao);

  const handleLancarNotas = () => {
    navigate(`/avaliacoes/${avaliacao.id}/lancar-notas`);
  };

  const handleDefinirGabarito = () => {
    navigate(`/avaliacoes/${avaliacao.id}/definir-gabarito`);
  };

  return (
    <div className="avaliacao-card">
      <div className="avaliacao-card-header">
        <div className="avaliacao-title-section">
          <h3 className="avaliacao-titulo">{avaliacao.titulo}</h3>
          <span className="avaliacao-disciplina">
            {avaliacao.disciplinaNome}
          </span>
        </div>
      </div>

      <div className="avaliacao-card-body">
        <div className="avaliacao-info-item">
          <span className="info-label">Data:</span>
          <span className="info-value">{formattedDate}</span>
        </div>
        
        <div className="avaliacao-info-item">
          <span className="info-label">Valor:</span>
          <span className="info-value">{avaliacao.valorMaximo.toFixed(1)} pontos</span>
        </div>

        <div className="avaliacao-info-item">
          <span className="info-label">Tipo:</span>
          <span className="info-value">{avaliacao.tipoDisplay}</span>
        </div>
      </div>

      <div className="avaliacao-card-actions">
        <button 
          className="btn-lancar-notas"
          onClick={handleLancarNotas}
        >
          <span className="btn-icon">✏️</span>
          Lançar Notas
        </button>
        {avaliacao.totalQuestoes > 0 ? (
          <button 
            className="btn-definir-gabarito"
            onClick={handleDefinirGabarito}
          >
            <span className="btn-icon">🔑</span>
            Definir Gabarito
          </button>
        ) : null}
      </div>
    </div>
  );
};

