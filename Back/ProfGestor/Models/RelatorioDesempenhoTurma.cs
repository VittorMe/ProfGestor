namespace ProfGestor.Models;

public class RelatorioDesempenhoTurma
{
    public long Id { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public double MediaGeral { get; set; }
    public double Mediana { get; set; }
    public int QtdAcimaMedia { get; set; }
    public int QtdAbaixoMedia { get; set; }
    public long TurmaId { get; set; }

    // Relacionamentos
    public Turma Turma { get; set; } = null!;
    public ICollection<LinhaRelatorioDesempenho> LinhasRelatorioDesempenho { get; set; } = new List<LinhaRelatorioDesempenho>();
}

