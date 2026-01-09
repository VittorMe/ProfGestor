namespace ProfGestor.DTOs;

public class GabaritoQuestaoDTO
{
    public long Id { get; set; }
    public char AlternativaCorreta { get; set; }
    public long QuestaoObjetivaId { get; set; }
    public int QuestaoNumero { get; set; }
}

public class GabaritoQuestaoCreateDTO
{
    public char AlternativaCorreta { get; set; }
    public long QuestaoObjetivaId { get; set; }
}

public class GabaritoQuestaoUpdateDTO
{
    public char AlternativaCorreta { get; set; }
}

public class DefinirGabaritoDTO
{
    public long AvaliacaoId { get; set; }
    public List<GabaritoItemDTO> Itens { get; set; } = new();
}

public class GabaritoItemDTO
{
    public long QuestaoObjetivaId { get; set; }
    public char? AlternativaCorreta { get; set; } // null se não foi definida
}

public class GabaritoResumoDTO
{
    public long AvaliacaoId { get; set; }
    public string AvaliacaoTitulo { get; set; } = string.Empty;
    public string DisciplinaNome { get; set; } = string.Empty;
    public string TurmaNome { get; set; } = string.Empty;
    public List<QuestaoGabaritoDTO> Questoes { get; set; } = new();
}

public class QuestaoGabaritoDTO
{
    public long Id { get; set; }
    public int Numero { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public double Valor { get; set; }
    public char? AlternativaCorreta { get; set; } // null se não tem gabarito definido
    public bool TemGabarito { get; set; }
}

