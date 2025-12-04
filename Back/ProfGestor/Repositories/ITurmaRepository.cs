using ProfGestor.Models;

namespace ProfGestor.Repositories;

public interface ITurmaRepository : IRepository<Turma>
{
    Task<Turma?> GetByIdWithDetailsAsync(long id);
    Task<IEnumerable<Turma>> GetByProfessorIdAsync(long professorId);
    Task<IEnumerable<Turma>> GetAllWithDetailsAsync();
}

