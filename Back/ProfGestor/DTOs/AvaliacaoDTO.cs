using ProfGestor.Models.Enums;

namespace ProfGestor.DTOs;

public class AvaliacaoDTO
{
    public long Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public TipoAvaliacao Tipo { get; set; }
    public string TipoDisplay { get; set; } = string.Empty; // Para exibição formatada
    public DateOnly DataAplicacao { get; set; }
    public double ValorMaximo { get; set; }
    public long DisciplinaId { get; set; }
    public string DisciplinaNome { get; set; } = string.Empty;
    public bool TemGabarito { get; set; }
    public bool TemNotasLancadas { get; set; }
    public int TotalQuestoes { get; set; }
}

public class QuestaoObjetivaCreateDTO
{
    public int Numero { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public double Valor { get; set; }
    public string? AlternativaCorreta { get; set; } // "A", "B", "C", "D" ou "E" (será convertido para char no service)
}

public class AvaliacaoCreateDTO
{
    public string Titulo { get; set; } = string.Empty;
    public TipoAvaliacao Tipo { get; set; }
    public DateOnly DataAplicacao { get; set; }
    public double ValorMaximo { get; set; }
    public long DisciplinaId { get; set; }
    public bool IsObjetiva { get; set; } // Indica se é avaliação objetiva
    public List<QuestaoObjetivaCreateDTO>? QuestoesObjetivas { get; set; } // Questões (apenas se IsObjetiva = true)
}

public class AvaliacaoUpdateDTO
{
    public string Titulo { get; set; } = string.Empty;
    public TipoAvaliacao Tipo { get; set; }
    public DateOnly DataAplicacao { get; set; }
    public double ValorMaximo { get; set; }
    public long DisciplinaId { get; set; }
}

