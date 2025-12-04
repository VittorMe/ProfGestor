namespace ProfGestor.DTOs;

public class AlunoDTO
{
    public long Id { get; set; }
    public string Matricula { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public long TurmaId { get; set; }
    public string TurmaNome { get; set; } = string.Empty;
}

public class AlunoCreateDTO
{
    public string Matricula { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public long TurmaId { get; set; }
}

public class AlunoUpdateDTO
{
    public string Matricula { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public long TurmaId { get; set; }
}

