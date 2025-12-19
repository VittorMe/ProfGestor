namespace ProfGestor.DTOs;

public class DashboardDTO
{
    public int TurmasAtivas { get; set; }
    public int TotalAlunos { get; set; }
    public int Planejamentos { get; set; }
    public int Avaliacoes { get; set; }
    public List<ProximaAulaDTO> ProximasAulas { get; set; } = new();
    public List<AtividadeRecenteDTO> AtividadesRecentes { get; set; } = new();
}

public class ProximaAulaDTO
{
    public string TurmaNome { get; set; } = string.Empty;
    public string DisciplinaNome { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public DateOnly Data { get; set; }
    public string Sala { get; set; } = string.Empty;
}

public class AtividadeRecenteDTO
{
    public string Acao { get; set; } = string.Empty;
    public string TurmaNome { get; set; } = string.Empty;
    public string DisciplinaNome { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
}

