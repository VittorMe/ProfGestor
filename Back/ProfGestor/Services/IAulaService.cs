using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface IAulaService
{
    Task<IEnumerable<AulaDTO>> GetAllAsync();
    Task<AulaDTO?> GetByIdAsync(long id);
    Task<IEnumerable<AulaDTO>> GetByTurmaIdAsync(long turmaId);
    Task<AulaDTO?> GetByTurmaIdAndDataAsync(long turmaId, DateOnly data);
    Task<AulaDTO> CreateAsync(AulaCreateDTO dto);
    Task<AulaDTO> UpdateAsync(long id, AulaUpdateDTO dto);
    Task<bool> DeleteAsync(long id);
}

