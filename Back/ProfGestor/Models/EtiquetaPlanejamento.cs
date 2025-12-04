namespace ProfGestor.Models;

public class EtiquetaPlanejamento
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public long PlanejamentoAulaId { get; set; }

    // Relacionamentos
    public PlanejamentoAula PlanejamentoAula { get; set; } = null!;
}

