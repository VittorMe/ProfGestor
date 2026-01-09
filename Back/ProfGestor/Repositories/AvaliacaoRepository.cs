using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.Models;

namespace ProfGestor.Repositories;

public class AvaliacaoRepository : Repository<Avaliacao>, IAvaliacaoRepository
{
    public AvaliacaoRepository(ProfGestorContext context) : base(context)
    {
    }

    public async Task<Avaliacao?> GetByIdWithDetailsAsync(long id)
    {
        return await _dbSet
            .Include(a => a.Disciplina)
            .Include(a => a.QuestoesObjetivas)
                .ThenInclude(q => q.GabaritoQuestao)
            .Include(a => a.NotasAvaliacao)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Avaliacao?> GetByIdWithQuestoesAsync(long id)
    {
        return await _dbSet
            .Include(a => a.Disciplina)
            .Include(a => a.QuestoesObjetivas.OrderBy(q => q.Numero))
                .ThenInclude(q => q.GabaritoQuestao)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Avaliacao>> GetByDisciplinaIdAsync(long disciplinaId)
    {
        return await _dbSet
            .Include(a => a.Disciplina)
            .Include(a => a.QuestoesObjetivas)
            .Where(a => a.DisciplinaId == disciplinaId)
            .OrderByDescending(a => a.DataAplicacao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Avaliacao>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(a => a.Disciplina)
            .Include(a => a.QuestoesObjetivas)
            .OrderByDescending(a => a.DataAplicacao)
            .ToListAsync();
    }

    public async Task<bool> ExistsByDisciplinaIdAndDataAsync(long disciplinaId, DateOnly dataAplicacao, long? excludeId = null)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(a => a.DisciplinaId == disciplinaId && a.DataAplicacao == dataAplicacao);
        
        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }
}

