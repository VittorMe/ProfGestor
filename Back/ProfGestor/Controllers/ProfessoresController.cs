using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfGestor.DTOs;
using ProfGestor.Services;

namespace ProfGestor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfessoresController : ControllerBase
{
    private readonly IProfessorService _professorService;

    public ProfessoresController(IProfessorService professorService)
    {
        _professorService = professorService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfessorDTO>>> GetAll()
    {
        var professores = await _professorService.GetAllAsync();
        return Ok(professores);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProfessorDTO>> GetById(long id)
    {
        var professor = await _professorService.GetByIdAsync(id);
        if (professor == null)
            return NotFound();

        return Ok(professor);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<ProfessorDTO>> GetByEmail(string email)
    {
        var professor = await _professorService.GetByEmailAsync(email);
        if (professor == null)
            return NotFound();

        return Ok(professor);
    }

    [HttpPost]
    public async Task<ActionResult<ProfessorDTO>> Create([FromBody] ProfessorCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _professorService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exceptions.BusinessException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProfessorDTO>> Update(long id, [FromBody] ProfessorUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _professorService.UpdateAsync(id, dto);
            return Ok(updated);
        }
        catch (Exceptions.NotFoundException)
        {
            return NotFound();
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
            var deleted = await _professorService.DeleteAsync(id);
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
