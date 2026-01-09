using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfGestor.DTOs;
using ProfGestor.Services;

namespace ProfGestor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlanejamentosAulaController : ControllerBase
{
    private readonly IPlanejamentoAulaService _planejamentoService;
    private readonly ITurmaService _turmaService;

    public PlanejamentosAulaController(IPlanejamentoAulaService planejamentoService, ITurmaService turmaService)
    {
        _planejamentoService = planejamentoService;
        _turmaService = turmaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlanejamentoAulaDTO>>> GetAll([FromQuery] string? search, [FromQuery] long? disciplinaId, [FromQuery] bool? favoritos)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        // Buscar disciplinas do professor através das turmas
        var turmas = await _turmaService.GetByProfessorIdAsync(professorId);
        var disciplinaIds = turmas.Select(t => t.DisciplinaId).Distinct().ToList();

        IEnumerable<PlanejamentoAulaDTO> planejamentos;

        // Se filtro de favoritos está ativo
        if (favoritos == true)
        {
            if (disciplinaId.HasValue && disciplinaIds.Contains(disciplinaId.Value))
            {
                // Favoritos de uma disciplina específica
                var todos = await _planejamentoService.GetByDisciplinaIdAsync(disciplinaId.Value);
                planejamentos = todos.Where(p => p.Favorito);
            }
            else
            {
                // Favoritos de todas as disciplinas do professor
                planejamentos = await _planejamentoService.GetFavoritosByDisciplinasAsync(disciplinaIds);
            }
        }
        else if (disciplinaId.HasValue && disciplinaIds.Contains(disciplinaId.Value))
        {
            // Filtro por disciplina específica
            planejamentos = await _planejamentoService.GetByDisciplinaIdAsync(disciplinaId.Value);
        }
        else
        {
            // Buscar por disciplinas do professor
            if (!string.IsNullOrWhiteSpace(search))
            {
                planejamentos = await _planejamentoService.SearchByDisciplinasAsync(search, disciplinaIds);
            }
            else
            {
                // Todos os planejamentos das disciplinas do professor
                var allPlanejamentos = await _planejamentoService.GetAllAsync();
                planejamentos = allPlanejamentos.Where(p => disciplinaIds.Contains(p.DisciplinaId));
            }
        }

        return Ok(planejamentos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlanejamentoAulaDTO>> GetById(long id)
    {
        var planejamento = await _planejamentoService.GetByIdAsync(id);
        if (planejamento == null)
            return NotFound();

        return Ok(planejamento);
    }

    [HttpGet("disciplina/{disciplinaId}")]
    public async Task<ActionResult<IEnumerable<PlanejamentoAulaDTO>>> GetByDisciplinaId(long disciplinaId)
    {
        try
        {
            var planejamentos = await _planejamentoService.GetByDisciplinaIdAsync(disciplinaId);
            return Ok(planejamentos);
        }
        catch (Exceptions.NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("favoritos")]
    public async Task<ActionResult<IEnumerable<PlanejamentoAulaDTO>>> GetFavoritos()
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        // Buscar disciplinas do professor através das turmas
        var turmas = await _turmaService.GetByProfessorIdAsync(professorId);
        var disciplinaIds = turmas.Select(t => t.DisciplinaId).Distinct().ToList();

        var planejamentos = await _planejamentoService.GetFavoritosByDisciplinasAsync(disciplinaIds);
        return Ok(planejamentos);
    }

    [HttpPost]
    public async Task<ActionResult<PlanejamentoAulaDTO>> Create([FromBody] PlanejamentoAulaCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _planejamentoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PlanejamentoAulaDTO>> Update(long id, [FromBody] PlanejamentoAulaUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _planejamentoService.UpdateAsync(id, dto);
            return Ok(updated);
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPatch("{id}/favorito")]
    public async Task<ActionResult<PlanejamentoAulaDTO>> ToggleFavorito(long id)
    {
        try
        {
            var updated = await _planejamentoService.ToggleFavoritoAsync(id);
            return Ok(updated);
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var deleted = await _planejamentoService.DeleteAsync(id);
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
