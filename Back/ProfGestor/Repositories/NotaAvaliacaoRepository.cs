using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.Models;

namespace ProfGestor.Repositories;

public class NotaAvaliacaoRepository : Repository<NotaAvaliacao>, INotaAvaliacaoRepository
{
    public NotaAvaliacaoRepository(ProfGestorContext context) : base(context)
    {
    }

    public async Task<IEnumerable<NotaAvaliacao>> GetByAvaliacaoIdAsync(long avaliacaoId)
    {
        return await _dbSet
            .Include(n => n.Aluno)
            .Include(n => n.Avaliacao)
            .Where(n => n.AvaliacaoId == avaliacaoId)
            .ToListAsync();
    }

    public async Task<NotaAvaliacao?> GetByAvaliacaoIdAndAlunoIdAsync(long avaliacaoId, long alunoId)
    {
        return await _dbSet
            .Include(n => n.Aluno)
            .Include(n => n.Avaliacao)
            .FirstOrDefaultAsync(n => n.AvaliacaoId == avaliacaoId && n.AlunoId == alunoId);
    }

    public async Task<IEnumerable<NotaAvaliacao>> GetByAlunoIdAsync(long alunoId)
    {
        return await _dbSet
            .Include(n => n.Avaliacao)
            .Where(n => n.AlunoId == alunoId)
            .ToListAsync();
    }
}

