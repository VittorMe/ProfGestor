using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface IPlanejamentoAulaService
{
    Task<IEnumerable<PlanejamentoAulaDTO>> GetAllAsync();
    Task<PlanejamentoAulaDTO?> GetByIdAsync(long id);
    Task<IEnumerable<PlanejamentoAulaDTO>> GetByDisciplinaIdAsync(long disciplinaId);
    Task<IEnumerable<PlanejamentoAulaDTO>> GetFavoritosAsync();
    Task<IEnumerable<PlanejamentoAulaDTO>> SearchAsync(string searchTerm);
    Task<IEnumerable<PlanejamentoAulaDTO>> GetFavoritosByDisciplinasAsync(IEnumerable<long> disciplinaIds);
    Task<IEnumerable<PlanejamentoAulaDTO>> SearchByDisciplinasAsync(string searchTerm, IEnumerable<long> disciplinaIds);
    Task<PlanejamentoAulaDTO> CreateAsync(PlanejamentoAulaCreateDTO dto);
    Task<PlanejamentoAulaDTO> UpdateAsync(long id, PlanejamentoAulaUpdateDTO dto);
    Task<PlanejamentoAulaDTO> ToggleFavoritoAsync(long id);
    Task<bool> DeleteAsync(long id);
}
