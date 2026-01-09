using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfGestor.DTOs;
using ProfGestor.Services;

namespace ProfGestor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GabaritosController : ControllerBase
{
    private readonly IGabaritoService _gabaritoService;

    public GabaritosController(IGabaritoService gabaritoService)
    {
        _gabaritoService = gabaritoService;
    }

    [HttpGet("avaliacao/{avaliacaoId}")]
    public async Task<ActionResult<GabaritoResumoDTO>> GetGabaritoResumo(long avaliacaoId)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        try
        {
            var resumo = await _gabaritoService.GetGabaritoResumoAsync(avaliacaoId, professorId);
            return Ok(resumo);
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost("definir")]
    public async Task<IActionResult> DefinirGabarito([FromBody] DefinirGabaritoDTO dto)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _gabaritoService.DefinirGabaritoAsync(dto, professorId);
            return Ok(new { message = "Gabarito salvo com sucesso" });
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exceptions.BadRequestException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            // Log detalhado do erro para debug
            Console.WriteLine($"Erro ao definir gabarito: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                Console.WriteLine($"InnerException StackTrace: {ex.InnerException.StackTrace}");
            }
            return StatusCode(500, new { error = $"Erro ao salvar gabarito: {ex.Message}" });
        }
    }
}

