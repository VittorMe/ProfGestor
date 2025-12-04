namespace ProfGestor.Models;

public class AnotacaoPlanejamento
{
    public long Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime DataRegistro { get; set; }
    public long PlanejamentoAulaId { get; set; }

    // Relacionamentos
    public PlanejamentoAula PlanejamentoAula { get; set; } = null!;
}

