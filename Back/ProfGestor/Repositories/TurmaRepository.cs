using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.Models;

namespace ProfGestor.Repositories;

public class TurmaRepository : Repository<Turma>, ITurmaRepository
{
    public TurmaRepository(ProfGestorContext context) : base(context)
    {
    }

    public async Task<Turma?> GetByIdWithDetailsAsync(long id)
    {
        return await _dbSet
            .Include(t => t.Professor)
            .Include(t => t.Disciplina)
            .Include(t => t.Alunos)
            .Include(t => t.Aulas)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Turma>> GetByProfessorIdAsync(long professorId)
    {
        return await _dbSet
            .Include(t => t.Disciplina)
            .Include(t => t.Alunos)
            .Where(t => t.ProfessorId == professorId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Turma>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(t => t.Professor)
            .Include(t => t.Disciplina)
            .Include(t => t.Alunos)
            .ToListAsync();
    }
}

