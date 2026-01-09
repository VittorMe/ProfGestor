using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfGestor.DTOs;
using ProfGestor.Services;

namespace ProfGestor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotasAvaliacaoController : ControllerBase
{
    private readonly INotaAvaliacaoService _notaService;

    public NotasAvaliacaoController(INotaAvaliacaoService notaService)
    {
        _notaService = notaService;
    }

    [HttpGet("avaliacao/{avaliacaoId}")]
    public async Task<ActionResult<LancamentoNotasResumoDTO>> GetLancamentoNotas(long avaliacaoId)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        try
        {
            var resumo = await _notaService.GetLancamentoNotasAsync(avaliacaoId, professorId);
            return Ok(resumo);
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }

    [HttpPost("lancar")]
    public async Task<IActionResult> LancarNotas([FromBody] LancarNotasDTO dto)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _notaService.LancarNotasAsync(dto, professorId);
            return Ok(new { message = "Notas lançadas com sucesso" });
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
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NotaAvaliacaoDTO>> GetById(long id)
    {
        var nota = await _notaService.GetByIdAsync(id);
        if (nota == null)
            return NotFound();

        return Ok(nota);
    }

    [HttpPost]
    public async Task<ActionResult<NotaAvaliacaoDTO>> Create([FromBody] NotaAvaliacaoCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _notaService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exceptions.BadRequestException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<NotaAvaliacaoDTO>> Update(long id, [FromBody] NotaAvaliacaoUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _notaService.UpdateAsync(id, dto);
            return Ok(updated);
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exceptions.BadRequestException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var deleted = await _notaService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exceptions.NotFoundException)
        {
            return NotFound();
        }
    }
}

