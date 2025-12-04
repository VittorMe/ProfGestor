using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfGestor.DTOs;
using ProfGestor.Services;

namespace ProfGestor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlunosController : ControllerBase
{
    private readonly IAlunoService _alunoService;

    public AlunosController(IAlunoService alunoService)
    {
        _alunoService = alunoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlunoDTO>>> GetAll()
    {
        var alunos = await _alunoService.GetAllAsync();
        return Ok(alunos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AlunoDTO>> GetById(long id)
    {
        var aluno = await _alunoService.GetByIdAsync(id);
        if (aluno == null)
            return NotFound();

        return Ok(aluno);
    }

    [HttpGet("turma/{turmaId}")]
    public async Task<ActionResult<IEnumerable<AlunoDTO>>> GetByTurmaId(long turmaId)
    {
        try
        {
            var alunos = await _alunoService.GetByTurmaIdAsync(turmaId);
            return Ok(alunos);
        }
        catch (Exceptions.NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("matricula/{matricula}")]
    public async Task<ActionResult<AlunoDTO>> GetByMatricula(string matricula)
    {
        var aluno = await _alunoService.GetByMatriculaAsync(matricula);
        if (aluno == null)
            return NotFound();

        return Ok(aluno);
    }

    [HttpPost]
    public async Task<ActionResult<AlunoDTO>> Create([FromBody] AlunoCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _alunoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
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

    [HttpPut("{id}")]
    public async Task<ActionResult<AlunoDTO>> Update(long id, [FromBody] AlunoUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _alunoService.UpdateAsync(id, dto);
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
            var deleted = await _alunoService.DeleteAsync(id);
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
