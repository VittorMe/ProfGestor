using ProfGestor.Models;

namespace ProfGestor.Repositories;

public interface IAlunoRepository : IRepository<Aluno>
{
    Task<Aluno?> GetByMatriculaAsync(string matricula);
    Task<Aluno?> GetByIdWithDetailsAsync(long id);
    Task<IEnumerable<Aluno>> GetByTurmaIdAsync(long turmaId);
    Task<IEnumerable<Aluno>> GetAllWithTurmaAsync();
}

