using ProfGestor.Models;

namespace ProfGestor.Repositories;

public interface IProfessorRepository : IRepository<Professor>
{
    Task<Professor?> GetByEmailAsync(string email);
    Task<Professor?> GetByIdWithTurmasAsync(long id);
}

