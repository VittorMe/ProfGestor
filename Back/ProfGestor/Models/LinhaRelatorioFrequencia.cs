namespace ProfGestor.Models;

public class LinhaRelatorioFrequencia
{
    public long Id { get; set; }
    public int TotalPresencas { get; set; }
    public int TotalFaltas { get; set; }
    public double PercentualAssiduidade { get; set; }
    public long RelatorioFrequenciaId { get; set; }
    public long AlunoId { get; set; }

    // Relacionamentos
    public RelatorioFrequencia RelatorioFrequencia { get; set; } = null!;
    public Aluno Aluno { get; set; } = null!;
}

