namespace ProfGestor.Models;

public class Aluno
{
    public long Id { get; set; }
    public string Matricula { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public long TurmaId { get; set; }

    // Relacionamentos
    public Turma Turma { get; set; } = null!;
    public ICollection<NotaAvaliacao> NotasAvaliacao { get; set; } = new List<NotaAvaliacao>();
    public ICollection<Frequencia> Frequencias { get; set; } = new List<Frequencia>();
    public ICollection<RespostaAluno> RespostasAluno { get; set; } = new List<RespostaAluno>();
    public ICollection<LinhaRelatorioDesempenho> LinhasRelatorioDesempenho { get; set; } = new List<LinhaRelatorioDesempenho>();
    public ICollection<LinhaRelatorioFrequencia> LinhasRelatorioFrequencia { get; set; } = new List<LinhaRelatorioFrequencia>();
}

