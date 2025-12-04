namespace ProfGestor.Models;

public class LinhaRelatorioDesempenho
{
    public long Id { get; set; }
    public double MediaAluno { get; set; }
    public long RelatorioDesempenhoTurmaId { get; set; }
    public long AlunoId { get; set; }

    // Relacionamentos
    public RelatorioDesempenhoTurma RelatorioDesempenhoTurma { get; set; } = null!;
    public Aluno Aluno { get; set; } = null!;
}

