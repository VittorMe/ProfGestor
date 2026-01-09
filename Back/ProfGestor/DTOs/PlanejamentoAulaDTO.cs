namespace ProfGestor.DTOs;

public class MaterialDidaticoDTO
{
    public long Id { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public string TipoMime { get; set; } = string.Empty;
    public double TamanhoMB { get; set; }
    public string UrlArquivo { get; set; } = string.Empty;
    public DateTime DataUpload { get; set; }
}

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
    public List<MaterialDidaticoDTO> MateriaisDidaticos { get; set; } = new();
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

