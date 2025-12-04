namespace ProfGestor.Models;

public class QuestaoObjetiva
{
    public long Id { get; set; }
    public int Numero { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public double Valor { get; set; }
    public long AvaliacaoId { get; set; }

    // Relacionamentos
    public Avaliacao Avaliacao { get; set; } = null!;
    public GabaritoQuestao? GabaritoQuestao { get; set; }
    public ICollection<RespostaAluno> RespostasAluno { get; set; } = new List<RespostaAluno>();
}

