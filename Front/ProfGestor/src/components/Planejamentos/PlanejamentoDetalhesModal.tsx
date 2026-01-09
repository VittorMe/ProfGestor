import { useEffect, useState } from 'react';
import { planejamentoService, type Planejamento } from '../../services/planejamentoService';
import { Loading } from '../UI/Loading';
import { ErrorMessage } from '../UI/ErrorMessage';
import './PlanejamentoDetalhesModal.css';

interface PlanejamentoDetalhesModalProps {
  planejamentoId: number;
  onClose: () => void;
  onToggleFavorito: () => void;
}

export const PlanejamentoDetalhesModal = ({ 
  planejamentoId, 
  onClose,
  onToggleFavorito 
}: PlanejamentoDetalhesModalProps) => {
  const [planejamento, setPlanejamento] = useState<Planejamento | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchPlanejamento = async () => {
      try {
        setLoading(true);
        setError(null);
        const data = await planejamentoService.getById(planejamentoId);
        setPlanejamento(data);
      } catch (err: any) {
        console.error('Erro ao carregar detalhes do planejamento:', err);
        setError('Erro ao carregar detalhes do planejamento. Tente novamente.');
      } finally {
        setLoading(false);
      }
    };

    fetchPlanejamento();
  }, [planejamentoId]);

  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  };

  if (loading) {
    return (
      <div className="modal-overlay" onClick={onClose}>
        <div className="modal-content" onClick={(e) => e.stopPropagation()}>
          <Loading message="Carregando detalhes do planejamento..." />
        </div>
      </div>
    );
  }

  if (error || !planejamento) {
    return (
      <div className="modal-overlay" onClick={onClose}>
        <div className="modal-content" onClick={(e) => e.stopPropagation()}>
          <div className="modal-header">
            <h2>Detalhes do Planejamento</h2>
            <button className="modal-close" onClick={onClose}>×</button>
          </div>
          <ErrorMessage message={error || 'Planejamento não encontrado'} onDismiss={onClose} />
        </div>
      </div>
    );
  }

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content planejamento-detalhes-modal" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <div className="modal-title-section">
            <h2>{planejamento.titulo}</h2>
            <span className="modal-disciplina">{planejamento.disciplinaNome}</span>
          </div>
          <div className="modal-header-actions">
            <button
              className={`favorito-btn-modal ${planejamento.favorito ? 'favorito' : ''}`}
              onClick={onToggleFavorito}
              title={planejamento.favorito ? 'Remover dos favoritos' : 'Adicionar aos favoritos'}
            >
              {planejamento.favorito ? '⭐' : '☆'}
            </button>
            <button className="modal-close" onClick={onClose}>×</button>
          </div>
        </div>

        <div className="modal-body">
          <div className="detalhes-grid">
            <div className="detalhe-item">
              <span className="detalhe-label">Data da Aula:</span>
              <span className="detalhe-value">{formatDate(planejamento.dataAula)}</span>
            </div>
            <div className="detalhe-item">
              <span className="detalhe-label">Disciplina:</span>
              <span className="detalhe-value">{planejamento.disciplinaNome}</span>
            </div>
            <div className="detalhe-item">
              <span className="detalhe-label">Criado em:</span>
              <span className="detalhe-value">
                {new Date(planejamento.criadoEm).toLocaleDateString('pt-BR')}
              </span>
            </div>
          </div>

          {planejamento.objetivos && (
            <section className="detalhes-section">
              <h3 className="section-title">Objetivos</h3>
              <p className="section-content">{planejamento.objetivos}</p>
            </section>
          )}

          {planejamento.conteudo && (
            <section className="detalhes-section">
              <h3 className="section-title">Conteúdo</h3>
              <div className="section-content" dangerouslySetInnerHTML={{ __html: planejamento.conteudo.replace(/\n/g, '<br />') }} />
            </section>
          )}

          {planejamento.metodologia && (
            <section className="detalhes-section">
              <h3 className="section-title">Metodologia</h3>
              <p className="section-content">{planejamento.metodologia}</p>
            </section>
          )}

          {planejamento.materiaisDidaticos && planejamento.materiaisDidaticos.length > 0 && (
            <section className="detalhes-section">
              <h3 className="section-title">
                Materiais Didáticos ({planejamento.materiaisDidaticos.length})
              </h3>
              <div className="materiais-list">
                {planejamento.materiaisDidaticos.map((material) => (
                  <div key={material.id} className="material-item">
                    <span className="material-icon">📎</span>
                    <div className="material-info">
                      <span className="material-nome">{material.nomeArquivo}</span>
                      <span className="material-tamanho">
                        {material.tamanhoMB.toFixed(2)} MB
                      </span>
                    </div>
                    <a 
                      href={material.urlArquivo} 
                      target="_blank" 
                      rel="noopener noreferrer"
                      className="material-download"
                    >
                      Download
                    </a>
                  </div>
                ))}
              </div>
            </section>
          )}
        </div>

        <div className="modal-footer">
          <button className="btn-close-modal" onClick={onClose}>
            Fechar
          </button>
        </div>
      </div>
    </div>
  );
};

