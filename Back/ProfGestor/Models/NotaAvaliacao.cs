namespace ProfGestor.Models;

public class NotaAvaliacao
{
    public long Id { get; set; }
    public double Valor { get; set; }
    public DateTime DataLancamento { get; set; }
    public string? Origem { get; set; }
    public long AlunoId { get; set; }
    public long AvaliacaoId { get; set; }

    // Relacionamentos
    public Aluno Aluno { get; set; } = null!;
    public Avaliacao Avaliacao { get; set; } = null!;
}

