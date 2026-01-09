using ProfGestor.Models;

namespace ProfGestor.Repositories;

public interface IGabaritoQuestaoRepository : IRepository<GabaritoQuestao>
{
    Task<GabaritoQuestao?> GetByQuestaoObjetivaIdAsync(long questaoObjetivaId);
    Task<IEnumerable<GabaritoQuestao>> GetByAvaliacaoIdAsync(long avaliacaoId);
}

