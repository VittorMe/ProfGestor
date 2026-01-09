using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.Models;

namespace ProfGestor.Repositories;

public class PlanejamentoAulaRepository : Repository<PlanejamentoAula>, IPlanejamentoAulaRepository
{
    public PlanejamentoAulaRepository(ProfGestorContext context) : base(context)
    {
    }

    public async Task<PlanejamentoAula?> GetByIdWithDetailsAsync(long id)
    {
        return await _dbSet
            .Include(p => p.Disciplina)
            .Include(p => p.MateriaisDidaticos)
            .Include(p => p.AnotacoesPlanejamento)
            .Include(p => p.EtiquetasPlanejamento)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<PlanejamentoAula>> GetByDisciplinaIdAsync(long disciplinaId)
    {
        return await _dbSet
            .Include(p => p.Disciplina)
            .Where(p => p.DisciplinaId == disciplinaId)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlanejamentoAula>> GetFavoritosAsync()
    {
        return await _dbSet
            .Include(p => p.Disciplina)
            .Where(p => p.Favorito)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlanejamentoAula>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(p => p.Disciplina)
            .Include(p => p.MateriaisDidaticos)
            .Include(p => p.AnotacoesPlanejamento)
            .Include(p => p.EtiquetasPlanejamento)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlanejamentoAula>> SearchAsync(string searchTerm)
    {
        var term = searchTerm;
        return await _dbSet
            .Include(p => p.Disciplina)
            .Where(p => 
                p.Titulo.Contains(term) ||
                (p.Objetivos != null && p.Objetivos.Contains(term)) ||
                (p.Conteudo != null && p.Conteudo.Contains(term))
            )
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlanejamentoAula>> GetFavoritosByDisciplinasAsync(IEnumerable<long> disciplinaIds)
    {
        return await _dbSet
            .Include(p => p.Disciplina)
            .Where(p => p.Favorito && disciplinaIds.Contains(p.DisciplinaId))
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlanejamentoAula>> SearchByDisciplinasAsync(string searchTerm, IEnumerable<long> disciplinaIds)
    {
        var term = searchTerm;
        return await _dbSet
            .Include(p => p.Disciplina)
            .Where(p => 
                disciplinaIds.Contains(p.DisciplinaId) &&
                (p.Titulo.Contains(term) ||
                 (p.Objetivos != null && p.Objetivos.Contains(term)) ||
                 (p.Conteudo != null && p.Conteudo.Contains(term)))
            )
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();
    }
}

