using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.Models;

namespace ProfGestor.Repositories;

public class GabaritoQuestaoRepository : Repository<GabaritoQuestao>, IGabaritoQuestaoRepository
{
    public GabaritoQuestaoRepository(ProfGestorContext context) : base(context)
    {
    }

    public async Task<GabaritoQuestao?> GetByQuestaoObjetivaIdAsync(long questaoObjetivaId)
    {
        return await _dbSet
            .Include(g => g.QuestaoObjetiva)
            .FirstOrDefaultAsync(g => g.QuestaoObjetivaId == questaoObjetivaId);
    }

    public async Task<IEnumerable<GabaritoQuestao>> GetByAvaliacaoIdAsync(long avaliacaoId)
    {
        return await _dbSet
            .Include(g => g.QuestaoObjetiva)
            .Where(g => g.QuestaoObjetiva.AvaliacaoId == avaliacaoId)
            .ToListAsync();
    }
}

