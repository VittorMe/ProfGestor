using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfGestor.DTOs;
using ProfGestor.Services;

namespace ProfGestor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AvaliacoesController : ControllerBase
{
    private readonly IAvaliacaoService _avaliacaoService;
    private readonly ITurmaService _turmaService;

    public AvaliacoesController(IAvaliacaoService avaliacaoService, ITurmaService turmaService)
    {
        _avaliacaoService = avaliacaoService;
        _turmaService = turmaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AvaliacaoDTO>>> GetAll([FromQuery] long? disciplinaId)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        // Buscar disciplinas do professor através das turmas
        var turmas = await _turmaService.GetByProfessorIdAsync(professorId);
        var disciplinaIds = turmas.Select(t => t.DisciplinaId).Distinct().ToList();

        IEnumerable<AvaliacaoDTO> avaliacoes;

        if (disciplinaId.HasValue && disciplinaIds.Contains(disciplinaId.Value))
        {
            // Filtro por disciplina específica
            avaliacoes = await _avaliacaoService.GetByDisciplinaIdAsync(disciplinaId.Value);
        }
        else
        {
            // Todas as avaliações das disciplinas do professor
            var allAvaliacoes = await _avaliacaoService.GetAllAsync();
            avaliacoes = allAvaliacoes.Where(a => disciplinaIds.Contains(a.DisciplinaId));
        }

        return Ok(avaliacoes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AvaliacaoDTO>> GetById(long id)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        var avaliacao = await _avaliacaoService.GetByIdAsync(id);
        if (avaliacao == null)
            return NotFound();

        // Verificar se a disciplina da avaliação pertence ao professor
        var turmas = await _turmaService.GetByProfessorIdAsync(professorId);
        var disciplinaIds = turmas.Select(t => t.DisciplinaId).Distinct().ToList();

        if (!disciplinaIds.Contains(avaliacao.DisciplinaId))
            return Forbid();

        return Ok(avaliacao);
    }

    [HttpPost]
    public async Task<ActionResult<AvaliacaoDTO>> Create([FromBody] AvaliacaoCreateDTO dto)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        // Verificar se a disciplina pertence ao professor
        var turmas = await _turmaService.GetByProfessorIdAsync(professorId);
        var disciplinaIds = turmas.Select(t => t.DisciplinaId).Distinct().ToList();

        if (!disciplinaIds.Contains(dto.DisciplinaId))
            return Forbid();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _avaliacaoService.CreateAsync(dto);
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
        catch (Exception ex)
        {
            // Log do erro completo para debug
            Console.WriteLine($"Erro ao criar avaliação: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"InnerException: {ex.InnerException.Message}");
            }
            return StatusCode(500, new { error = $"Erro ao criar avaliação: {ex.Message}" });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AvaliacaoDTO>> Update(long id, [FromBody] AvaliacaoUpdateDTO dto)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        // Verificar se a avaliação existe e pertence ao professor
        var avaliacaoExistente = await _avaliacaoService.GetByIdAsync(id);
        if (avaliacaoExistente == null)
            return NotFound();

        var turmas = await _turmaService.GetByProfessorIdAsync(professorId);
        var disciplinaIds = turmas.Select(t => t.DisciplinaId).Distinct().ToList();

        if (!disciplinaIds.Contains(avaliacaoExistente.DisciplinaId) || 
            !disciplinaIds.Contains(dto.DisciplinaId))
            return Forbid();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _avaliacaoService.UpdateAsync(id, dto);
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
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        // Verificar se a avaliação existe e pertence ao professor
        var avaliacao = await _avaliacaoService.GetByIdAsync(id);
        if (avaliacao == null)
            return NotFound();

        var turmas = await _turmaService.GetByProfessorIdAsync(professorId);
        var disciplinaIds = turmas.Select(t => t.DisciplinaId).Distinct().ToList();

        if (!disciplinaIds.Contains(avaliacao.DisciplinaId))
            return Forbid();

        try
        {
            var deleted = await _avaliacaoService.DeleteAsync(id);
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

