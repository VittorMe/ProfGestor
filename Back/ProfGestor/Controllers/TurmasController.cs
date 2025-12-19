using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfGestor.DTOs;
using ProfGestor.Services;

namespace ProfGestor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TurmasController : ControllerBase
{
    private readonly ITurmaService _turmaService;

    public TurmasController(ITurmaService turmaService)
    {
        _turmaService = turmaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TurmaDTO>>> GetAll()
    {
        // Pegar professorId do token JWT
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        // Filtrar turmas do professor logado
        var turmas = await _turmaService.GetByProfessorIdAsync(professorId);
        return Ok(turmas);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TurmaDTO>> GetById(long id)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        var turma = await _turmaService.GetByIdAsync(id);
        if (turma == null)
            return NotFound();

        // Validar se a turma pertence ao professor
        if (turma.ProfessorId != professorId)
            return Forbid();

        return Ok(turma);
    }

    [HttpGet("professor/{professorId}")]
    public async Task<ActionResult<IEnumerable<TurmaDTO>>> GetByProfessorId(long professorId)
    {
        try
        {
            var turmas = await _turmaService.GetByProfessorIdAsync(professorId);
            return Ok(turmas);
        }
        catch (Exceptions.NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<TurmaDTO>> Create([FromBody] TurmaCreateDTO dto)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        // Garantir que o professorId seja o do professor logado
        dto.ProfessorId = professorId;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _turmaService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TurmaDTO>> Update(long id, [FromBody] TurmaUpdateDTO dto)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        // Verificar se a turma existe e pertence ao professor
        var turmaExistente = await _turmaService.GetByIdAsync(id);
        if (turmaExistente == null)
            return NotFound();

        if (turmaExistente.ProfessorId != professorId)
            return Forbid();

        // Garantir que o professorId no DTO seja o do professor logado
        dto.ProfessorId = professorId;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _turmaService.UpdateAsync(id, dto);
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
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        // Verificar se a turma existe e pertence ao professor
        var turma = await _turmaService.GetByIdAsync(id);
        if (turma == null)
            return NotFound();

        if (turma.ProfessorId != professorId)
            return Forbid();

        try
        {
            var deleted = await _turmaService.DeleteAsync(id);
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
