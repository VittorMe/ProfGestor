using ProfGestor.Models;

namespace ProfGestor.Repositories;

public interface INotaAvaliacaoRepository : IRepository<NotaAvaliacao>
{
    Task<IEnumerable<NotaAvaliacao>> GetByAvaliacaoIdAsync(long avaliacaoId);
    Task<NotaAvaliacao?> GetByAvaliacaoIdAndAlunoIdAsync(long avaliacaoId, long alunoId);
    Task<IEnumerable<NotaAvaliacao>> GetByAlunoIdAsync(long alunoId);
}

