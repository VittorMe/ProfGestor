using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.Models;

namespace ProfGestor.Repositories;

public class AlunoRepository : Repository<Aluno>, IAlunoRepository
{
    public AlunoRepository(ProfGestorContext context) : base(context)
    {
    }

    public async Task<Aluno?> GetByMatriculaAsync(string matricula)
    {
        return await _dbSet
            .Include(a => a.Turma)
            .FirstOrDefaultAsync(a => a.Matricula == matricula);
    }

    public async Task<Aluno?> GetByIdWithDetailsAsync(long id)
    {
        return await _dbSet
            .Include(a => a.Turma)
            .Include(a => a.NotasAvaliacao)
            .Include(a => a.Frequencias)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Aluno>> GetByTurmaIdAsync(long turmaId)
    {
        return await _dbSet
            .Include(a => a.Turma)
            .Where(a => a.TurmaId == turmaId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Aluno>> GetAllWithTurmaAsync()
    {
        return await _dbSet
            .Include(a => a.Turma)
            .ToListAsync();
    }
}

