namespace ProfGestor.DTOs;

public class TurmaDTO
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int AnoLetivo { get; set; }
    public int Semestre { get; set; }
    public string Turno { get; set; } = string.Empty;
    public int QtdAlunos { get; set; }
    public long ProfessorId { get; set; }
    public string ProfessorNome { get; set; } = string.Empty;
    public long DisciplinaId { get; set; }
    public string DisciplinaNome { get; set; } = string.Empty;
    public int TotalAlunos { get; set; }
}

public class TurmaCreateDTO
{
    public string Nome { get; set; } = string.Empty;
    public int AnoLetivo { get; set; }
    public int Semestre { get; set; }
    public string Turno { get; set; } = string.Empty;
    public int QtdAlunos { get; set; }
    public long ProfessorId { get; set; }
    public long DisciplinaId { get; set; }
}

public class TurmaUpdateDTO
{
    public string Nome { get; set; } = string.Empty;
    public int AnoLetivo { get; set; }
    public int Semestre { get; set; }
    public string Turno { get; set; } = string.Empty;
    public int QtdAlunos { get; set; }
    public long ProfessorId { get; set; }
    public long DisciplinaId { get; set; }
}

