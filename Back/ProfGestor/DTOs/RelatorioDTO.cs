namespace ProfGestor.DTOs;

public class RelatorioFrequenciaRequestDTO
{
    public long TurmaId { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly DataFim { get; set; }
}

public class AlunoFrequenciaRelatorioDTO
{
    public long AlunoId { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public int TotalAulas { get; set; }
    public int Presencas { get; set; }
    public int Faltas { get; set; }
    public int FaltasJustificadas { get; set; }
    public double PercentualPresenca { get; set; }
}

public class RelatorioFrequenciaDTO
{
    public long TurmaId { get; set; }
    public string TurmaNome { get; set; } = string.Empty;
    public string DisciplinaNome { get; set; } = string.Empty;
    public DateOnly DataInicio { get; set; }
    public DateOnly DataFim { get; set; }
    public DateTime GeradoEm { get; set; }
    public List<AlunoFrequenciaRelatorioDTO> Alunos { get; set; } = new();
    public int TotalAulas { get; set; }
    public double MediaPresenca { get; set; }
    public int TotalPresencas { get; set; }
    public int TotalFaltas { get; set; }
    public int TotalFaltasJustificadas { get; set; }
}

public class RelatorioDesempenhoRequestDTO
{
    public long TurmaId { get; set; }
    public string? Periodo { get; set; } // Opcional: "1º Bimestre", "2º Bimestre", etc.
}

public class AlunoDesempenhoRelatorioDTO
{
    public long AlunoId { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public int TotalAvaliacoes { get; set; }
    public double MediaGeral { get; set; }
    public double SomaNotas { get; set; }
    public double SomaValorMaximo { get; set; }
    public List<AvaliacaoDesempenhoDTO> Avaliacoes { get; set; } = new();
}

public class AvaliacaoDesempenhoDTO
{
    public long AvaliacaoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public DateOnly DataAplicacao { get; set; }
    public double ValorMaximo { get; set; }
    public double? Nota { get; set; }
    public double? Percentual { get; set; }
}

public class DistribuicaoNotasDTO
{
    public string Faixa { get; set; } = string.Empty; // "0-3", "3-5", "5-7", "7-9", "9-10"
    public int Quantidade { get; set; }
}

public class ClassificacaoDesempenhoDTO
{
    public string Categoria { get; set; } = string.Empty; // "Insuficiente", "Regular", "Bom", "Excelente"
    public string Faixa { get; set; } = string.Empty; // "0-5", "5-7", "7-9", "9-10"
    public int Quantidade { get; set; }
    public double Percentual { get; set; }
}

public class RelatorioDesempenhoDTO
{
    public long TurmaId { get; set; }
    public string TurmaNome { get; set; } = string.Empty;
    public string DisciplinaNome { get; set; } = string.Empty;
    public string? Periodo { get; set; }
    public DateTime GeradoEm { get; set; }
    public List<AlunoDesempenhoRelatorioDTO> Alunos { get; set; } = new();
    public double MediaGeralTurma { get; set; }
    public double MedianaTurma { get; set; }
    public double MaiorNota { get; set; }
    public double MenorNota { get; set; }
    public int QtdAcimaMedia { get; set; }
    public int QtdAbaixoMedia { get; set; }
    public List<DistribuicaoNotasDTO> DistribuicaoNotas { get; set; } = new();
    public List<ClassificacaoDesempenhoDTO> ClassificacaoDesempenho { get; set; } = new();
    public string? Observacao { get; set; }
    public string? Recomendacao { get; set; }
}
