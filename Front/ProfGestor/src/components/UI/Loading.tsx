import './Loading.css';

interface LoadingProps {
  message?: string;
}

export const Loading = ({ message = 'Carregando...' }: LoadingProps) => {
  return (
    <div className="loading-container">
      <div className="loading-spinner"></div>
      <p className="loading-message">{message}</p>
    </div>
  );
};


