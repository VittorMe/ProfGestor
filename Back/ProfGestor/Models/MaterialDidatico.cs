namespace ProfGestor.Models;

public class MaterialDidatico
{
    public long Id { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public string TipoMime { get; set; } = string.Empty;
    public double TamanhoMB { get; set; }
    public string UrlArquivo { get; set; } = string.Empty;
    public DateTime DataUpload { get; set; }
    public long PlanejamentoAulaId { get; set; }

    // Relacionamentos
    public PlanejamentoAula PlanejamentoAula { get; set; } = null!;
}

