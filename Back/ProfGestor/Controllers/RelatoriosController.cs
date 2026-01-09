using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfGestor.DTOs;
using ProfGestor.Services;

namespace ProfGestor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;
    private readonly ITurmaService _turmaService;

    public RelatoriosController(IRelatorioService relatorioService, ITurmaService turmaService)
    {
        _relatorioService = relatorioService;
        _turmaService = turmaService;
    }

    [HttpPost("frequencia")]
    public async Task<ActionResult<RelatorioFrequenciaDTO>> GerarRelatorioFrequencia([FromBody] RelatorioFrequenciaRequestDTO request)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        // Verificar se a turma pertence ao professor
        var turmas = await _turmaService.GetByProfessorIdAsync(professorId);
        if (!turmas.Any(t => t.Id == request.TurmaId))
            return Forbid();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var relatorio = await _relatorioService.GerarRelatorioFrequenciaAsync(request);
            return Ok(relatorio);
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exceptions.BadRequestException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao gerar relatório de frequência: {ex.Message}");
            return StatusCode(500, new { error = $"Erro ao gerar relatório: {ex.Message}" });
        }
    }

    [HttpPost("desempenho")]
    public async Task<ActionResult<RelatorioDesempenhoDTO>> GerarRelatorioDesempenho([FromBody] RelatorioDesempenhoRequestDTO request)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        // Verificar se a turma pertence ao professor
        var turmas = await _turmaService.GetByProfessorIdAsync(professorId);
        if (!turmas.Any(t => t.Id == request.TurmaId))
            return Forbid();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var relatorio = await _relatorioService.GerarRelatorioDesempenhoAsync(request);
            return Ok(relatorio);
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exceptions.BadRequestException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao gerar relatório de desempenho: {ex.Message}");
            return StatusCode(500, new { error = $"Erro ao gerar relatório: {ex.Message}" });
        }
    }
}
