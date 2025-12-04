using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.DTOs;
using ProfGestor.Models;

namespace ProfGestor.Repositories;

public class LogRepository : Repository<Log>, ILogRepository
{
    public LogRepository(ProfGestorContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Log> Logs, int Total)> GetLogsFiltradosAsync(LogFiltroDTO filtro)
    {
        var query = _dbSet.AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrEmpty(filtro.Nivel))
            query = query.Where(l => l.Nivel == filtro.Nivel);

        if (!string.IsNullOrEmpty(filtro.Usuario))
            query = query.Where(l => l.Usuario != null && l.Usuario.Contains(filtro.Usuario));

        if (filtro.DataInicio.HasValue)
            query = query.Where(l => l.DataHora >= filtro.DataInicio.Value);

        if (filtro.DataFim.HasValue)
            query = query.Where(l => l.DataHora <= filtro.DataFim.Value);

        if (!string.IsNullOrEmpty(filtro.Endpoint))
            query = query.Where(l => l.Endpoint != null && l.Endpoint.Contains(filtro.Endpoint));

        // Contar total
        var total = await query.CountAsync();

        // Aplicar paginação e ordenação
        var logs = await query
            .OrderByDescending(l => l.DataHora)
            .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
            .Take(filtro.TamanhoPagina)
            .ToListAsync();

        return (logs, total);
    }

    public async Task<LogResumoDTO> GetResumoAsync()
    {
        var totalLogs = await _dbSet.CountAsync();
        var totalErros = await _dbSet.CountAsync(l => l.Nivel == "Error");
        var totalWarnings = await _dbSet.CountAsync(l => l.Nivel == "Warning");
        var totalInformacoes = await _dbSet.CountAsync(l => l.Nivel == "Information");
        var ultimoLog = await _dbSet
            .OrderByDescending(l => l.DataHora)
            .Select(l => l.DataHora)
            .FirstOrDefaultAsync();

        return new LogResumoDTO
        {
            TotalLogs = totalLogs,
            TotalErros = totalErros,
            TotalWarnings = totalWarnings,
            TotalInformacoes = totalInformacoes,
            UltimoLog = ultimoLog
        };
    }

    public async Task<bool> LimparLogsAntigosAsync(DateTime dataLimite)
    {
        var logsAntigos = await _dbSet
            .Where(l => l.DataHora < dataLimite)
            .ToListAsync();

        if (logsAntigos.Any())
        {
            await DeleteRangeAsync(logsAntigos);
            return true;
        }

        return false;
    }
}

