namespace ProfGestor.DTOs;

public class PlanejamentoAulaDTO
{
    public long Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public DateOnly DataAula { get; set; }
    public string? Objetivos { get; set; }
    public string? Conteudo { get; set; }
    public string? Metodologia { get; set; }
    public bool Favorito { get; set; }
    public DateTime CriadoEm { get; set; }
    public long DisciplinaId { get; set; }
    public string DisciplinaNome { get; set; } = string.Empty;
}

public class PlanejamentoAulaCreateDTO
{
    public string Titulo { get; set; } = string.Empty;
    public DateOnly DataAula { get; set; }
    public string? Objetivos { get; set; }
    public string? Conteudo { get; set; }
    public string? Metodologia { get; set; }
    public bool Favorito { get; set; }
    public long DisciplinaId { get; set; }
}

public class PlanejamentoAulaUpdateDTO
{
    public string Titulo { get; set; } = string.Empty;
    public DateOnly DataAula { get; set; }
    public string? Objetivos { get; set; }
    public string? Conteudo { get; set; }
    public string? Metodologia { get; set; }
    public bool Favorito { get; set; }
    public long DisciplinaId { get; set; }
}

