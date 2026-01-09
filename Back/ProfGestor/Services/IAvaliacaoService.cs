using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface IAvaliacaoService
{
    Task<IEnumerable<AvaliacaoDTO>> GetAllAsync();
    Task<AvaliacaoDTO?> GetByIdAsync(long id);
    Task<IEnumerable<AvaliacaoDTO>> GetByDisciplinaIdAsync(long disciplinaId);
    Task<AvaliacaoDTO> CreateAsync(AvaliacaoCreateDTO dto);
    Task<AvaliacaoDTO> UpdateAsync(long id, AvaliacaoUpdateDTO dto);
    Task<bool> DeleteAsync(long id);
}

