namespace ProfGestor.Models;

public class Aula
{
    public long Id { get; set; }
    public DateOnly Data { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public long TurmaId { get; set; }

    // Relacionamentos
    public Turma Turma { get; set; } = null!;
    public ICollection<Frequencia> Frequencias { get; set; } = new List<Frequencia>();
    public AnotacaoAula? AnotacaoAula { get; set; }
}

