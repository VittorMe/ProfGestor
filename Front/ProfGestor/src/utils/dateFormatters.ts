export const formatDate = (dateString: string): string => {
  const date = new Date(dateString);
  const today = new Date();
  const yesterday = new Date(today);
  yesterday.setDate(yesterday.getDate() - 1);

  if (date.toDateString() === today.toDateString()) {
    return `Hoje, ${date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}`;
  } else if (date.toDateString() === yesterday.toDateString()) {
    return `Ontem, ${date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}`;
  } else {
    return date.toLocaleDateString('pt-BR', { 
      day: '2-digit', 
      month: '2-digit',
      hour: '2-digit', 
      minute: '2-digit' 
    });
  }
};

export const formatClassTime = (periodo: string, data: string): string => {
  const date = new Date(data);
  const timeParts = periodo.split('-');
  
  // Se o período já contém horário (formato "HH:MM - HH:MM"), usar diretamente
  if (timeParts.length >= 2 && timeParts[0].trim().match(/^\d{1,2}:\d{2}$/)) {
    return `${timeParts[0].trim()} - ${timeParts[1].trim()}`;
  }
  
  // Caso contrário, retornar apenas o período
  return periodo || date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
};

