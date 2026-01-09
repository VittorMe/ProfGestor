using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProfGestor.DTOs;
using ProfGestor.Services;

namespace ProfGestor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AulasController : ControllerBase
{
    private readonly IAulaService _aulaService;
    private readonly ITurmaService _turmaService;

    public AulasController(IAulaService aulaService, ITurmaService turmaService)
    {
        _aulaService = aulaService;
        _turmaService = turmaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AulaDTO>>> GetAll()
    {
        var aulas = await _aulaService.GetAllAsync();
        return Ok(aulas);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AulaDTO>> GetById(long id)
    {
        var aula = await _aulaService.GetByIdAsync(id);
        if (aula == null)
            return NotFound();

        return Ok(aula);
    }

    [HttpGet("turma/{turmaId}")]
    public async Task<ActionResult<IEnumerable<AulaDTO>>> GetByTurmaId(long turmaId)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        try
        {
            // Verificar se a turma pertence ao professor
            var turma = await _turmaService.GetByIdAsync(turmaId);
            if (turma == null)
                return NotFound();

            if (turma.ProfessorId != professorId)
                return Forbid();

            var aulas = await _aulaService.GetByTurmaIdAsync(turmaId);
            return Ok(aulas);
        }
        catch (Exceptions.NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("turma/{turmaId}/data/{data}")]
    public async Task<ActionResult<AulaDTO>> GetByTurmaIdAndData(long turmaId, [ModelBinder(typeof(ProfGestor.Binders.DateOnlyModelBinder))] DateOnly data)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        try
        {
            // Verificar se a turma pertence ao professor
            var turma = await _turmaService.GetByIdAsync(turmaId);
            if (turma == null)
                return NotFound();

            if (turma.ProfessorId != professorId)
                return Forbid();

            var aula = await _aulaService.GetByTurmaIdAndDataAsync(turmaId, data);
            if (aula == null)
                return NotFound();

            return Ok(aula);
        }
        catch (Exceptions.NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<AulaDTO>> Create([FromBody] AulaCreateDTO dto)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        // Verificar se a turma pertence ao professor
        var turma = await _turmaService.GetByIdAsync(dto.TurmaId);
        if (turma == null)
            return NotFound(new { error = "Turma não encontrada" });

        if (turma.ProfessorId != professorId)
            return Forbid();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _aulaService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AulaDTO>> Update(long id, [FromBody] AulaUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _aulaService.UpdateAsync(id, dto);
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
            var deleted = await _aulaService.DeleteAsync(id);
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

