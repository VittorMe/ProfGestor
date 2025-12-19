using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.Models;

namespace ProfGestor.Repositories;

public class AulaRepository : Repository<Aula>, IAulaRepository
{
    public AulaRepository(ProfGestorContext context) : base(context)
    {
    }

    public async Task<Aula?> GetByIdWithDetailsAsync(long id)
    {
        return await _dbSet
            .Include(a => a.Turma)
                .ThenInclude(t => t.Disciplina)
            .Include(a => a.Frequencias)
                .ThenInclude(f => f.Aluno)
            .Include(a => a.AnotacaoAula)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Aula>> GetByTurmaIdAsync(long turmaId)
    {
        return await _dbSet
            .Include(a => a.Turma)
                .ThenInclude(t => t.Disciplina)
            .Include(a => a.Frequencias)
            .Where(a => a.TurmaId == turmaId)
            .OrderByDescending(a => a.Data)
            .ToListAsync();
    }

    public async Task<IEnumerable<Aula>> GetByTurmaIdAndDataAsync(long turmaId, DateOnly data)
    {
        return await _dbSet
            .Include(a => a.Turma)
            .Include(a => a.Frequencias)
            .Where(a => a.TurmaId == turmaId && a.Data == data)
            .ToListAsync();
    }

    public async Task<IEnumerable<Aula>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(a => a.Turma)
                .ThenInclude(t => t.Disciplina)
            .Include(a => a.Frequencias)
            .ToListAsync();
    }
}

