using ProfGestor.Models;

namespace ProfGestor.Repositories;

public interface IFrequenciaRepository : IRepository<Frequencia>
{
    Task<IEnumerable<Frequencia>> GetByAulaIdAsync(long aulaId);
    Task<IEnumerable<Frequencia>> GetByAlunoIdAsync(long alunoId);
    Task<Frequencia?> GetByAulaIdAndAlunoIdAsync(long aulaId, long alunoId);
    Task<IEnumerable<Frequencia>> GetByAulaIdWithDetailsAsync(long aulaId);
}


