using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfGestor.DTOs;
using ProfGestor.Services;

namespace ProfGestor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LogsController : ControllerBase
{
    private readonly ILogService _logService;

    public LogsController(ILogService logService)
    {
        _logService = logService;
    }

    [HttpGet]
    public async Task<ActionResult<object>> GetLogs([FromQuery] LogFiltroDTO filtro)
    {
        var (logs, total) = await _logService.GetLogsFiltradosAsync(filtro);
        
        return Ok(new
        {
            Logs = logs,
            Total = total,
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
            TotalPaginas = (int)Math.Ceiling(total / (double)filtro.TamanhoPagina)
        });
    }

    [HttpGet("resumo")]
    public async Task<ActionResult<LogResumoDTO>> GetResumo()
    {
        var resumo = await _logService.GetResumoAsync();
        return Ok(resumo);
    }

    [HttpDelete("limpar")]
    [Authorize(Roles = "Admin")] // Ajuste conforme sua necessidade
    public async Task<IActionResult> LimparLogsAntigos([FromQuery] int diasParaManter = 30)
    {
        if (diasParaManter < 1)
            return BadRequest(new { error = "Dias para manter deve ser maior que 0" });

        var sucesso = await _logService.LimparLogsAntigosAsync(diasParaManter);
        if (!sucesso)
            return NotFound(new { message = "Nenhum log antigo encontrado para remover" });

        return Ok(new { message = $"Logs anteriores a {diasParaManter} dias foram removidos" });
    }
}




