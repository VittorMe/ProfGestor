namespace ProfGestor.Models;

public class AnotacaoAula
{
    public long Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime DataRegistro { get; set; }
    public long AulaId { get; set; }

    // Relacionamentos
    public Aula Aula { get; set; } = null!;
}

