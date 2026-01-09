import jsPDF from 'jspdf';
import type { RelatorioFrequencia, RelatorioDesempenho } from '../services/relatorioService';

export const exportarRelatorioFrequenciaPDF = (relatorio: RelatorioFrequencia) => {
  const doc = new jsPDF();
  const pageWidth = doc.internal.pageSize.getWidth();
  const pageHeight = doc.internal.pageSize.getHeight();
  let yPos = 20;
  const margin = 20;
  const lineHeight = 7;

  // Cabeçalho
  doc.setFontSize(18);
  doc.setTextColor(26, 54, 93);
  doc.text('Relatório de Frequência', margin, yPos);
  yPos += lineHeight + 2;

  doc.setFontSize(12);
  doc.setTextColor(0, 0, 0);
  doc.text(`${relatorio.turmaNome || ''} • ${relatorio.disciplinaNome || ''}`, margin, yPos);
  yPos += lineHeight;

  const dataInicio = new Date(relatorio.dataInicio).toLocaleDateString('pt-BR');
  const dataFim = new Date(relatorio.dataFim).toLocaleDateString('pt-BR');
  doc.text(`Período: ${dataInicio} a ${dataFim}`, margin, yPos);
  yPos += lineHeight + 5;

  // Resumo
  doc.setFontSize(14);
  doc.setFont('helvetica', 'bold');
  doc.text('Resumo da Turma', margin, yPos);
  yPos += lineHeight + 2;

  doc.setFontSize(11);
  doc.setFont('helvetica', 'normal');
  doc.text(`Total de Aulas: ${relatorio.totalAulas || 0}`, margin, yPos);
  yPos += lineHeight;
  doc.text(`Média de Assiduidade: ${(relatorio.mediaPresenca || 0).toFixed(1)}%`, margin, yPos);
  yPos += lineHeight;
  doc.text(`Total de Presenças: ${relatorio.totalPresencas || 0}`, margin, yPos);
  yPos += lineHeight;
  doc.text(`Total de Faltas: ${relatorio.totalFaltas || 0}`, margin, yPos);
  yPos += lineHeight;
  doc.text(`Faltas Justificadas: ${relatorio.totalFaltasJustificadas || 0}`, margin, yPos);
  yPos += lineHeight + 5;

  // Tabela
  doc.setFontSize(14);
  doc.setFont('helvetica', 'bold');
  doc.text('Detalhamento por Aluno', margin, yPos);
  yPos += lineHeight + 3;

  // Cabeçalho da tabela
  doc.setFontSize(10);
  doc.setFont('helvetica', 'bold');
  const colWidths = [70, 25, 20, 25, 30];
  const headers = ['Aluno', 'Presenças', 'Faltas', 'Justificadas', 'Assiduidade'];
  let xPos = margin;

  headers.forEach((header, index) => {
    doc.text(header, xPos, yPos);
    xPos += colWidths[index];
  });
  yPos += lineHeight + 2;

  // Linha separadora
  doc.setLineWidth(0.5);
  doc.line(margin, yPos - 2, pageWidth - margin, yPos - 2);
  yPos += 2;

  // Dados da tabela
  doc.setFont('helvetica', 'normal');
  relatorio.alunos.forEach((aluno) => {
    // Verificar se precisa de nova página
    if (yPos > pageHeight - 30) {
      doc.addPage();
      yPos = 20;
    }

    xPos = margin;
    doc.text((aluno.alunoNome || '').substring(0, 30), xPos, yPos);
    xPos += colWidths[0];
    doc.text((aluno.presencas || 0).toString(), xPos, yPos);
    xPos += colWidths[1];
    doc.text((aluno.faltas || 0).toString(), xPos, yPos);
    xPos += colWidths[2];
    doc.text((aluno.faltasJustificadas || 0).toString(), xPos, yPos);
    xPos += colWidths[3];
    doc.text(`${(aluno.percentualPresenca || 0).toFixed(1)}%`, xPos, yPos);
    yPos += lineHeight;
  });

  // Rodapé
  const totalPages = doc.getNumberOfPages();
  for (let i = 1; i <= totalPages; i++) {
    doc.setPage(i);
    doc.setFontSize(8);
    doc.setTextColor(128, 128, 128);
    doc.text(
      `Gerado em ${new Date(relatorio.geradoEm).toLocaleString('pt-BR')} - Página ${i} de ${totalPages}`,
      margin,
      pageHeight - 10
    );
  }

  // Salvar PDF
  const fileName = `Relatorio_Frequencia_${(relatorio.turmaNome || 'Turma').replace(/\s+/g, '_')}_${dataInicio.replace(/\//g, '-')}_${dataFim.replace(/\//g, '-')}.pdf`;
  doc.save(fileName);
};

export const exportarRelatorioDesempenhoPDF = (relatorio: RelatorioDesempenho) => {
  const doc = new jsPDF();
  const pageWidth = doc.internal.pageSize.getWidth();
  const pageHeight = doc.internal.pageSize.getHeight();
  let yPos = 20;
  const margin = 20;
  const lineHeight = 7;

  // Cabeçalho
  doc.setFontSize(18);
  doc.setTextColor(26, 54, 93);
  doc.text('Relatório de Desempenho', margin, yPos);
  yPos += lineHeight + 2;

  doc.setFontSize(12);
  doc.setTextColor(0, 0, 0);
  doc.text(`${relatorio.turmaNome || ''} • ${relatorio.disciplinaNome || ''}`, margin, yPos);
  yPos += lineHeight;

  if (relatorio.periodo) {
    doc.text(`Período: ${relatorio.periodo}`, margin, yPos);
    yPos += lineHeight;
  }
  yPos += 5;

  // Resumo
  doc.setFontSize(14);
  doc.setFont('helvetica', 'bold');
  doc.text('Resumo da Turma', margin, yPos);
  yPos += lineHeight + 2;

  doc.setFontSize(11);
  doc.setFont('helvetica', 'normal');
  doc.text(`Média Geral: ${(relatorio.mediaGeralTurma || 0).toFixed(1)}%`, margin, yPos);
  yPos += lineHeight;
  doc.text(`Mediana: ${(relatorio.medianaTurma || 0).toFixed(1)}%`, margin, yPos);
  yPos += lineHeight;
  doc.text(`Maior Nota: ${(relatorio.maiorNota || 0).toFixed(1)}`, margin, yPos);
  yPos += lineHeight;
  doc.text(`Menor Nota: ${(relatorio.menorNota || 0).toFixed(1)}`, margin, yPos);
  yPos += lineHeight;
  doc.text(`Acima da Média: ${relatorio.qtdAcimaMedia || 0} alunos`, margin, yPos);
  yPos += lineHeight;
  doc.text(`Abaixo da Média: ${relatorio.qtdAbaixoMedia || 0} alunos`, margin, yPos);
  yPos += lineHeight + 5;

  // Distribuição de Notas
  if (relatorio.distribuicaoNotas && relatorio.distribuicaoNotas.length > 0) {
    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.text('Distribuição de Notas', margin, yPos);
    yPos += lineHeight + 3;

    doc.setFontSize(10);
    doc.setFont('helvetica', 'normal');
    relatorio.distribuicaoNotas.forEach((item) => {
      doc.text(`${item.faixa || ''}: ${item.quantidade || 0} alunos`, margin + 10, yPos);
      yPos += lineHeight;
    });
    yPos += 5;
  }

  // Classificação de Desempenho
  if (relatorio.classificacaoDesempenho && relatorio.classificacaoDesempenho.length > 0) {
    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.text('Classificação de Desempenho', margin, yPos);
    yPos += lineHeight + 3;

    doc.setFontSize(10);
    doc.setFont('helvetica', 'normal');
    relatorio.classificacaoDesempenho.forEach((item) => {
      doc.text(`${item.categoria || ''} (${item.faixa || ''}): ${item.quantidade || 0} alunos (${(item.percentual || 0).toFixed(1)}%)`, margin + 10, yPos);
      yPos += lineHeight;
    });
    yPos += 5;
  }

  // Tabela de Alunos
  doc.setFontSize(14);
  doc.setFont('helvetica', 'bold');
  doc.text('Desempenho por Aluno', margin, yPos);
  yPos += lineHeight + 3;

  // Cabeçalho da tabela
  doc.setFontSize(10);
  doc.setFont('helvetica', 'bold');
  const colWidths = [80, 40, 40];
  const headers = ['Aluno', 'Média Geral', 'Total Avaliações'];
  let xPos = margin;

  headers.forEach((header, index) => {
    doc.text(header, xPos, yPos);
    xPos += colWidths[index];
  });
  yPos += lineHeight + 2;

  // Linha separadora
  doc.setLineWidth(0.5);
  doc.line(margin, yPos - 2, pageWidth - margin, yPos - 2);
  yPos += 2;

  // Dados da tabela
  doc.setFont('helvetica', 'normal');
  relatorio.alunos.forEach((aluno) => {
    // Verificar se precisa de nova página
    if (yPos > pageHeight - 30) {
      doc.addPage();
      yPos = 20;
    }

    xPos = margin;
    doc.text((aluno.alunoNome || '').substring(0, 35), xPos, yPos);
    xPos += colWidths[0];
    doc.text(`${(aluno.mediaGeral || 0).toFixed(1)}%`, xPos, yPos);
    xPos += colWidths[1];
    doc.text((aluno.totalAvaliacoes || 0).toString(), xPos, yPos);
    yPos += lineHeight;
  });

  // Análise e Recomendações
  if (relatorio.observacao || relatorio.recomendacao) {
    if (yPos > pageHeight - 50) {
      doc.addPage();
      yPos = 20;
    } else {
      yPos += 10;
    }

    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.text('Análise e Recomendações', margin, yPos);
    yPos += lineHeight + 3;

    doc.setFontSize(10);
    doc.setFont('helvetica', 'normal');
    if (relatorio.observacao) {
      doc.setFont('helvetica', 'bold');
      doc.text('Observação:', margin, yPos);
      yPos += lineHeight;
      doc.setFont('helvetica', 'normal');
      const observacaoLines = doc.splitTextToSize(relatorio.observacao || '', pageWidth - 2 * margin);
      doc.text(observacaoLines, margin, yPos);
      yPos += observacaoLines.length * lineHeight + 3;
    }

    if (relatorio.recomendacao) {
      doc.setFont('helvetica', 'bold');
      doc.text('Recomendação:', margin, yPos);
      yPos += lineHeight;
      doc.setFont('helvetica', 'normal');
      const recomendacaoLines = doc.splitTextToSize(relatorio.recomendacao || '', pageWidth - 2 * margin);
      doc.text(recomendacaoLines, margin, yPos);
      yPos += recomendacaoLines.length * lineHeight;
    }
  }

  // Rodapé
  const totalPages = doc.getNumberOfPages();
  for (let i = 1; i <= totalPages; i++) {
    doc.setPage(i);
    doc.setFontSize(8);
    doc.setTextColor(128, 128, 128);
    doc.text(
      `Gerado em ${new Date(relatorio.geradoEm).toLocaleString('pt-BR')} - Página ${i} de ${totalPages}`,
      margin,
      pageHeight - 10
    );
  }

  // Salvar PDF
  const periodoStr = relatorio.periodo ? relatorio.periodo.replace(/\s+/g, '_') : 'Geral';
  const fileName = `Relatorio_Desempenho_${(relatorio.turmaNome || 'Turma').replace(/\s+/g, '_')}_${periodoStr}.pdf`;
  doc.save(fileName);
};
