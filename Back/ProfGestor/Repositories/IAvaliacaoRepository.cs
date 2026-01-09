using ProfGestor.Models;

namespace ProfGestor.Repositories;

public interface IAvaliacaoRepository : IRepository<Avaliacao>
{
    Task<Avaliacao?> GetByIdWithDetailsAsync(long id);
    Task<Avaliacao?> GetByIdWithQuestoesAsync(long id);
    Task<IEnumerable<Avaliacao>> GetByDisciplinaIdAsync(long disciplinaId);
    Task<IEnumerable<Avaliacao>> GetAllWithDetailsAsync();
    Task<bool> ExistsByDisciplinaIdAndDataAsync(long disciplinaId, DateOnly dataAplicacao, long? excludeId = null);
}

