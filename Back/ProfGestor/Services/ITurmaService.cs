using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface ITurmaService
{
    Task<IEnumerable<TurmaDTO>> GetAllAsync();
    Task<TurmaDTO?> GetByIdAsync(long id);
    Task<IEnumerable<TurmaDTO>> GetByProfessorIdAsync(long professorId);
    Task<TurmaDTO> CreateAsync(TurmaCreateDTO dto);
    Task<TurmaDTO> UpdateAsync(long id, TurmaUpdateDTO dto);
    Task<bool> DeleteAsync(long id);
}
