namespace ProfGestor.DTOs;

public class AulaDTO
{
    public long Id { get; set; }
    public DateOnly Data { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public long TurmaId { get; set; }
    public string TurmaNome { get; set; } = string.Empty;
    public string DisciplinaNome { get; set; } = string.Empty;
    public bool TemFrequenciaRegistrada { get; set; }
    public string? AnotacaoTexto { get; set; }
}

public class AulaCreateDTO
{
    public DateOnly Data { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public long TurmaId { get; set; }
}

public class AulaUpdateDTO
{
    public DateOnly Data { get; set; }
    public string Periodo { get; set; } = string.Empty;
}

