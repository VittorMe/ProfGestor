using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface IDisciplinaService
{
    Task<IEnumerable<DisciplinaDTO>> GetAllAsync();
    Task<DisciplinaDTO?> GetByIdAsync(long id);
    Task<DisciplinaDTO> CreateAsync(DisciplinaCreateDTO dto);
    Task<DisciplinaDTO> UpdateAsync(long id, DisciplinaUpdateDTO dto);
    Task<bool> DeleteAsync(long id);
}

