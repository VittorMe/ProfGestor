using ProfGestor.Models.Enums;

namespace ProfGestor.DTOs;

public class FrequenciaDTO
{
    public long Id { get; set; }
    public StatusFrequencia Status { get; set; }
    public long AlunoId { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public string AlunoMatricula { get; set; } = string.Empty;
    public long AulaId { get; set; }
}

public class FrequenciaCreateDTO
{
    public StatusFrequencia Status { get; set; }
    public long AlunoId { get; set; }
    public long AulaId { get; set; }
}

public class FrequenciaUpdateDTO
{
    public StatusFrequencia Status { get; set; }
}

public class RegistrarFrequenciaDTO
{
    public long TurmaId { get; set; }
    public DateOnly DataAula { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public List<FrequenciaAlunoDTO> Frequencias { get; set; } = new();
    public string? AnotacaoTexto { get; set; }
}

public class FrequenciaAlunoDTO
{
    public long AlunoId { get; set; }
    public StatusFrequencia Status { get; set; }
}


