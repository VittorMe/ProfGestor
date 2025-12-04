namespace ProfGestor.Models;

public class Log
{
    public long Id { get; set; }
    public string Nivel { get; set; } = string.Empty; // Information, Warning, Error, etc.
    public string Mensagem { get; set; } = string.Empty;
    public string? Excecao { get; set; }
    public string? StackTrace { get; set; }
    public string? Usuario { get; set; } // Email ou ID do usuário
    public string? Endpoint { get; set; } // Rota da API
    public string? MetodoHttp { get; set; } // GET, POST, etc.
    public DateTime DataHora { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}




