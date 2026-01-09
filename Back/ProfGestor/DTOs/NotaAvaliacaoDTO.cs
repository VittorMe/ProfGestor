namespace ProfGestor.DTOs;

public class NotaAvaliacaoDTO
{
    public long Id { get; set; }
    public double Valor { get; set; }
    public DateTime DataLancamento { get; set; }
    public string? Origem { get; set; }
    public long AlunoId { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public string AlunoMatricula { get; set; } = string.Empty;
    public long AvaliacaoId { get; set; }
}

public class NotaAvaliacaoCreateDTO
{
    public double Valor { get; set; }
    public string? Origem { get; set; }
    public long AlunoId { get; set; }
    public long AvaliacaoId { get; set; }
}

public class NotaAvaliacaoUpdateDTO
{
    public double Valor { get; set; }
    public string? Origem { get; set; }
}

public class LancarNotasDTO
{
    public long AvaliacaoId { get; set; }
    public List<NotaAlunoDTO> Notas { get; set; } = new();
}

public class NotaAlunoDTO
{
    public long AlunoId { get; set; }
    public double Valor { get; set; }
}

public class AlunoNotaDTO
{
    public long AlunoId { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public string AlunoMatricula { get; set; } = string.Empty;
    public string Iniciais { get; set; } = string.Empty;
    public double? Nota { get; set; }
    public bool TemNota { get; set; }
    public DateTime? DataLancamento { get; set; }
}

public class LancamentoNotasResumoDTO
{
    public long AvaliacaoId { get; set; }
    public string AvaliacaoTitulo { get; set; } = string.Empty;
    public string DisciplinaNome { get; set; } = string.Empty;
    public string TurmaNome { get; set; } = string.Empty;
    public double ValorMaximo { get; set; }
    public double MediaTurma { get; set; }
    public int NotasLancadas { get; set; }
    public int TotalAlunos { get; set; }
    public List<AlunoNotaDTO> Alunos { get; set; } = new();
}

