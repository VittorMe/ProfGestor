using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface IPlanejamentoAulaService
{
    Task<IEnumerable<PlanejamentoAulaDTO>> GetAllAsync();
    Task<PlanejamentoAulaDTO?> GetByIdAsync(long id);
    Task<IEnumerable<PlanejamentoAulaDTO>> GetByDisciplinaIdAsync(long disciplinaId);
    Task<IEnumerable<PlanejamentoAulaDTO>> GetFavoritosAsync();
    Task<PlanejamentoAulaDTO> CreateAsync(PlanejamentoAulaCreateDTO dto);
    Task<PlanejamentoAulaDTO> UpdateAsync(long id, PlanejamentoAulaUpdateDTO dto);
    Task<bool> DeleteAsync(long id);
}
