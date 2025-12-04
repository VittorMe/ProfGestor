namespace ProfGestor.Models;

public class GabaritoQuestao
{
    public long Id { get; set; }
    public char AlternativaCorreta { get; set; }
    public long QuestaoObjetivaId { get; set; }

    // Relacionamentos
    public QuestaoObjetiva QuestaoObjetiva { get; set; } = null!;
}

