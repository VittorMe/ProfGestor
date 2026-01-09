using ProfGestor.Models;

namespace ProfGestor.Repositories;

public interface IPlanejamentoAulaRepository : IRepository<PlanejamentoAula>
{
    Task<PlanejamentoAula?> GetByIdWithDetailsAsync(long id);
    Task<IEnumerable<PlanejamentoAula>> GetByDisciplinaIdAsync(long disciplinaId);
    Task<IEnumerable<PlanejamentoAula>> GetFavoritosAsync();
    Task<IEnumerable<PlanejamentoAula>> GetAllWithDetailsAsync();
    Task<IEnumerable<PlanejamentoAula>> SearchAsync(string searchTerm);
    Task<IEnumerable<PlanejamentoAula>> GetFavoritosByDisciplinasAsync(IEnumerable<long> disciplinaIds);
    Task<IEnumerable<PlanejamentoAula>> SearchByDisciplinasAsync(string searchTerm, IEnumerable<long> disciplinaIds);
}

