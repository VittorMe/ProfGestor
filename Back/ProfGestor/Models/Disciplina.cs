namespace ProfGestor.Models;

public class Disciplina
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    // Relacionamentos
    public Turma? Turma { get; set; }
    public ICollection<PlanejamentoAula> PlanejamentosAula { get; set; } = new List<PlanejamentoAula>();
    public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
}

