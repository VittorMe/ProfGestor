namespace ProfGestor.Models;

public class RelatorioFrequencia
{
    public long Id { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly DataFim { get; set; }
    public DateTime GeradoEm { get; set; }
    public long TurmaId { get; set; }

    // Relacionamentos
    public Turma Turma { get; set; } = null!;
    public ICollection<LinhaRelatorioFrequencia> LinhasRelatorioFrequencia { get; set; } = new List<LinhaRelatorioFrequencia>();
}

