using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.Models;

namespace ProfGestor.Repositories;

public class FrequenciaRepository : Repository<Frequencia>, IFrequenciaRepository
{
    public FrequenciaRepository(ProfGestorContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Frequencia>> GetByAulaIdAsync(long aulaId)
    {
        return await _dbSet
            .Where(f => f.AulaId == aulaId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Frequencia>> GetByAlunoIdAsync(long alunoId)
    {
        return await _dbSet
            .Where(f => f.AlunoId == alunoId)
            .ToListAsync();
    }

    public async Task<Frequencia?> GetByAulaIdAndAlunoIdAsync(long aulaId, long alunoId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(f => f.AulaId == aulaId && f.AlunoId == alunoId);
    }

    public async Task<IEnumerable<Frequencia>> GetByAulaIdWithDetailsAsync(long aulaId)
    {
        return await _dbSet
            .Include(f => f.Aluno)
            .Include(f => f.Aula)
                .ThenInclude(a => a.Turma)
            .Where(f => f.AulaId == aulaId)
            .OrderBy(f => f.Aluno.Nome)
            .ToListAsync();
    }
}

