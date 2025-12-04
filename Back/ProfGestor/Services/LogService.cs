using AutoMapper;
using ProfGestor.DTOs;
using ProfGestor.Repositories;

namespace ProfGestor.Services;

public class LogService : ILogService
{
    private readonly ILogRepository _repository;
    private readonly IMapper _mapper;

    public LogService(ILogRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task RegistrarLogAsync(string nivel, string mensagem, Exception? excecao = null, string? usuario = null, string? endpoint = null, string? metodoHttp = null, string? ipAddress = null, string? userAgent = null)
    {
        var log = new Models.Log
        {
            Nivel = nivel,
            Mensagem = mensagem,
            Excecao = excecao?.Message,
            StackTrace = excecao?.StackTrace,
            Usuario = usuario,
            Endpoint = endpoint,
            MetodoHttp = metodoHttp,
            DataHora = DateTime.Now,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        await _repository.AddAsync(log);
    }

    public async Task<(IEnumerable<LogDTO> Logs, int Total)> GetLogsFiltradosAsync(LogFiltroDTO filtro)
    {
        var (logs, total) = await _repository.GetLogsFiltradosAsync(filtro);
        var logsDTO = _mapper.Map<IEnumerable<LogDTO>>(logs);
        return (logsDTO, total);
    }

    public async Task<LogResumoDTO> GetResumoAsync()
    {
        return await _repository.GetResumoAsync();
    }

    public async Task<bool> LimparLogsAntigosAsync(int diasParaManter)
    {
        var dataLimite = DateTime.Now.AddDays(-diasParaManter);
        return await _repository.LimparLogsAntigosAsync(dataLimite);
    }
}




