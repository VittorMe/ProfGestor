using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfGestor.DTOs;
using ProfGestor.Services;

namespace ProfGestor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DisciplinasController : ControllerBase
{
    private readonly IDisciplinaService _disciplinaService;

    public DisciplinasController(IDisciplinaService disciplinaService)
    {
        _disciplinaService = disciplinaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DisciplinaDTO>>> GetAll()
    {
        var disciplinas = await _disciplinaService.GetAllAsync();
        return Ok(disciplinas);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DisciplinaDTO>> GetById(long id)
    {
        var disciplina = await _disciplinaService.GetByIdAsync(id);
        if (disciplina == null)
            return NotFound();

        return Ok(disciplina);
    }

    [HttpPost]
    public async Task<ActionResult<DisciplinaDTO>> Create([FromBody] DisciplinaCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _disciplinaService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exceptions.BusinessException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DisciplinaDTO>> Update(long id, [FromBody] DisciplinaUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _disciplinaService.UpdateAsync(id, dto);
            return Ok(updated);
        }
        catch (Exceptions.NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exceptions.BusinessException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var deleted = await _disciplinaService.DeleteAsync(id);
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


