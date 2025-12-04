using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface IProfessorService
{
    Task<IEnumerable<ProfessorDTO>> GetAllAsync();
    Task<ProfessorDTO?> GetByIdAsync(long id);
    Task<ProfessorDTO?> GetByEmailAsync(string email);
    Task<ProfessorDTO> CreateAsync(ProfessorCreateDTO dto);
    Task<ProfessorDTO> UpdateAsync(long id, ProfessorUpdateDTO dto);
    Task<bool> DeleteAsync(long id);
}
