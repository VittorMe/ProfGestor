using ProfGestor.Models;

namespace ProfGestor.Repositories;

public interface IAulaRepository : IRepository<Aula>
{
    Task<Aula?> GetByIdWithDetailsAsync(long id);
    Task<IEnumerable<Aula>> GetByTurmaIdAsync(long turmaId);
    Task<IEnumerable<Aula>> GetByTurmaIdAndDataAsync(long turmaId, DateOnly data);
    Task<IEnumerable<Aula>> GetAllWithDetailsAsync();
}

