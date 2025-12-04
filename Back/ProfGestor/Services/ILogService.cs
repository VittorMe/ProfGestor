using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface ILogService
{
    Task RegistrarLogAsync(string nivel, string mensagem, Exception? excecao = null, string? usuario = null, string? endpoint = null, string? metodoHttp = null, string? ipAddress = null, string? userAgent = null);
    Task<(IEnumerable<LogDTO> Logs, int Total)> GetLogsFiltradosAsync(LogFiltroDTO filtro);
    Task<LogResumoDTO> GetResumoAsync();
    Task<bool> LimparLogsAntigosAsync(int diasParaManter);
}




