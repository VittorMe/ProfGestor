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
        var turmas = await _turmaService.GetAllAsync();
        return Ok(turmas);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TurmaDTO>> GetById(long id)
    {
        var turma = await _turmaService.GetByIdAsync(id);
        if (turma == null)
            return NotFound();

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
