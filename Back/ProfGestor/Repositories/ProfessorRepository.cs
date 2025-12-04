using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.Models;

namespace ProfGestor.Repositories;

public class ProfessorRepository : Repository<Professor>, IProfessorRepository
{
    public ProfessorRepository(ProfGestorContext context) : base(context)
    {
    }

    public async Task<Professor?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Email == email);
    }

    public async Task<Professor?> GetByIdWithTurmasAsync(long id)
    {
        return await _dbSet
            .Include(p => p.Turmas)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}

