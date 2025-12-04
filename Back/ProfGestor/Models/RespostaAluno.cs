namespace ProfGestor.Models;

public class RespostaAluno
{
    public long Id { get; set; }
    public char AlternativaMarcada { get; set; }
    public bool Correta { get; set; }
    public long AlunoId { get; set; }
    public long QuestaoObjetivaId { get; set; }

    // Relacionamentos
    public Aluno Aluno { get; set; } = null!;
    public QuestaoObjetiva QuestaoObjetiva { get; set; } = null!;
}

