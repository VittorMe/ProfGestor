namespace ProfGestor.Models;

public class PlanejamentoAula
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

    // Relacionamentos
    public Disciplina Disciplina { get; set; } = null!;
    public ICollection<MaterialDidatico> MateriaisDidaticos { get; set; } = new List<MaterialDidatico>();
    public ICollection<AnotacaoPlanejamento> AnotacoesPlanejamento { get; set; } = new List<AnotacaoPlanejamento>();
    public ICollection<EtiquetaPlanejamento> EtiquetasPlanejamento { get; set; } = new List<EtiquetaPlanejamento>();
}

