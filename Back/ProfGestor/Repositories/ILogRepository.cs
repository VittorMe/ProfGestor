using ProfGestor.DTOs;
using ProfGestor.Models;

namespace ProfGestor.Repositories;

public interface ILogRepository : IRepository<Log>
{
    Task<(IEnumerable<Log> Logs, int Total)> GetLogsFiltradosAsync(LogFiltroDTO filtro);
    Task<LogResumoDTO> GetResumoAsync();
    Task<bool> LimparLogsAntigosAsync(DateTime dataLimite);
}




