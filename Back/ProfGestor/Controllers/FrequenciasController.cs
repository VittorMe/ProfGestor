using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfGestor.DTOs;
using ProfGestor.Services;

namespace ProfGestor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FrequenciasController : ControllerBase
{
    private readonly IFrequenciaService _frequenciaService;
    private readonly ITurmaService _turmaService;
    private readonly IAulaService _aulaService;

    public FrequenciasController(IFrequenciaService frequenciaService, ITurmaService turmaService, IAulaService aulaService)
    {
        _frequenciaService = frequenciaService;
        _turmaService = turmaService;
        _aulaService = aulaService;
    }

    [HttpGet("aula/{aulaId}")]
    public async Task<ActionResult<IEnumerable<FrequenciaDTO>>> GetByAulaId(long aulaId)
    {
        var professorId = long.Parse(User.FindFirst("ProfessorId")?.Value ?? "0");
        if (professorId == 0)
            return Unauthorized();

        // Buscar a aula para verificar a turma
        var aula = await _aulaService.GetByIdAsync(aulaId);
        if (aula == null)
            return NotFound();

        // Verificar se a turma pertence ao professor
        var turma = await _turmaService.GetByIdAsync(aula.TurmaId);
        if (turma == null)
            return NotFound();

        if (turma.ProfessorId != professorId)
            return Forbid();

        var frequencias = await _frequenciaService.GetByAulaIdAsync(aulaId);
        return Ok(frequencias);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FrequenciaDTO>> GetById(long id)
    {
        var frequencia = await _frequenciaService.GetByIdAsync(id);
        if (frequencia == null)
            return NotFound();

        return Ok(frequencia);
    }

    [HttpPost]
    public async Task<ActionResult<FrequenciaDTO>> Create([FromBody] FrequenciaCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _frequenciaService.CreateAsync(dto);
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

    [HttpPost("registrar")]
    public async Task<ActionResult<AulaDTO>> RegistrarFrequencia([FromBody] RegistrarFrequenciaDTO dto)
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
            var aula = await _frequenciaService.RegistrarFrequenciaAsync(dto);
            return Ok(aula);
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
    public async Task<ActionResult<FrequenciaDTO>> Update(long id, [FromBody] FrequenciaUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _frequenciaService.UpdateAsync(id, dto);
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
            var deleted = await _frequenciaService.DeleteAsync(id);
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

