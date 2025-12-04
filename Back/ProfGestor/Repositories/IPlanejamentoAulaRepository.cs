using ProfGestor.Models;

namespace ProfGestor.Repositories;

public interface IPlanejamentoAulaRepository : IRepository<PlanejamentoAula>
{
    Task<PlanejamentoAula?> GetByIdWithDetailsAsync(long id);
    Task<IEnumerable<PlanejamentoAula>> GetByDisciplinaIdAsync(long disciplinaId);
    Task<IEnumerable<PlanejamentoAula>> GetFavoritosAsync();
    Task<IEnumerable<PlanejamentoAula>> GetAllWithDetailsAsync();
}

