using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface IAlunoService
{
    Task<IEnumerable<AlunoDTO>> GetAllAsync();
    Task<AlunoDTO?> GetByIdAsync(long id);
    Task<IEnumerable<AlunoDTO>> GetByTurmaIdAsync(long turmaId);
    Task<AlunoDTO?> GetByMatriculaAsync(string matricula);
    Task<AlunoDTO> CreateAsync(AlunoCreateDTO dto);
    Task<AlunoDTO> UpdateAsync(long id, AlunoUpdateDTO dto);
    Task<bool> DeleteAsync(long id);
}
