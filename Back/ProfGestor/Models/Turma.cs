namespace ProfGestor.Models;

public class Turma
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int AnoLetivo { get; set; }
    public int Semestre { get; set; }
    public string Turno { get; set; } = string.Empty;
    public int QtdAlunos { get; set; }
    public long ProfessorId { get; set; }
    public long DisciplinaId { get; set; }

    // Relacionamentos
    public Professor Professor { get; set; } = null!;
    public Disciplina Disciplina { get; set; } = null!;
    public ICollection<Aluno> Alunos { get; set; } = new List<Aluno>();
    public ICollection<Aula> Aulas { get; set; } = new List<Aula>();
    public RelatorioDesempenhoTurma? RelatorioDesempenhoTurma { get; set; }
    public RelatorioFrequencia? RelatorioFrequencia { get; set; }
}

