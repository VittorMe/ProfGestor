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

    public PlanejamentosAulaController(IPlanejamentoAulaService planejamentoService)
    {
        _planejamentoService = planejamentoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlanejamentoAulaDTO>>> GetAll()
    {
        var planejamentos = await _planejamentoService.GetAllAsync();
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
        var planejamentos = await _planejamentoService.GetFavoritosAsync();
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
