using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface IGabaritoService
{
    Task<GabaritoResumoDTO> GetGabaritoResumoAsync(long avaliacaoId, long professorId);
    Task DefinirGabaritoAsync(DefinirGabaritoDTO dto, long professorId);
}

