namespace ProfGestor.DTOs;

public class LogDTO
{
    public long Id { get; set; }
    public string Nivel { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string? Excecao { get; set; }
    public string? StackTrace { get; set; }
    public string? Usuario { get; set; }
    public string? Endpoint { get; set; }
    public string? MetodoHttp { get; set; }
    public DateTime DataHora { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

public class LogFiltroDTO
{
    public string? Nivel { get; set; }
    public string? Usuario { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? Endpoint { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 50;
}

public class LogResumoDTO
{
    public int TotalLogs { get; set; }
    public int TotalErros { get; set; }
    public int TotalWarnings { get; set; }
    public int TotalInformacoes { get; set; }
    public DateTime? UltimoLog { get; set; }
}




