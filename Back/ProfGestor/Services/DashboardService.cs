using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.DTOs;
using ProfGestor.Repositories;

namespace ProfGestor.Services;

public class DashboardService : IDashboardService
{
    private readonly ITurmaRepository _turmaRepository;
    private readonly IPlanejamentoAulaRepository _planejamentoRepository;
    private readonly ProfGestorContext _context;

    public DashboardService(
        ITurmaRepository turmaRepository,
        IPlanejamentoAulaRepository planejamentoRepository,
        ProfGestorContext context)
    {
        _turmaRepository = turmaRepository;
        _planejamentoRepository = planejamentoRepository;
        _context = context;
    }

    public async Task<DashboardDTO> GetDashboardDataAsync(long professorId)
    {
        // Buscar todas as turmas do professor
        var turmas = await _turmaRepository.GetByProfessorIdAsync(professorId);
        var turmasList = turmas.ToList();
        
        // Obter IDs das disciplinas das turmas do professor
        var disciplinaIds = turmasList.Select(t => t.DisciplinaId).Distinct().ToList();
        
        // Contar turmas ativas
        var turmasAtivas = turmasList.Count;
        
        // Contar total de alunos (soma de todos os alunos das turmas)
        var totalAlunos = turmasList.Sum(t => t.Alunos.Count);
        
        // Contar planejamentos das disciplinas do professor
        var planejamentos = await _context.PlanejamentosAula
            .Where(p => disciplinaIds.Contains(p.DisciplinaId))
            .CountAsync();
        
        // Contar avaliações das disciplinas do professor
        var avaliacoes = await _context.Avaliacoes
            .Where(a => disciplinaIds.Contains(a.DisciplinaId))
            .CountAsync();
        
        // Buscar próximas aulas (próximas 4 aulas das turmas do professor)
        var hoje = DateOnly.FromDateTime(DateTime.Now);
        var turmaIds = turmasList.Select(t => t.Id).ToList();
        var proximasAulas = await _context.Aulas
            .Include(a => a.Turma)
                .ThenInclude(t => t.Disciplina)
            .Where(a => turmaIds.Contains(a.TurmaId) && a.Data >= hoje)
            .OrderBy(a => a.Data)
            .ThenBy(a => a.Periodo)
            .Take(4)
            .Select(a => new ProximaAulaDTO
            {
                TurmaNome = a.Turma.Nome,
                DisciplinaNome = a.Turma.Disciplina.Nome,
                Periodo = a.Periodo,
                Data = a.Data,
                Sala = "Sala " + (a.TurmaId % 10 + 1) // Placeholder - ajustar conforme necessário
            })
            .ToListAsync();
        
        // Buscar atividades recentes (últimas notas lançadas e frequências registradas)
        var atividadesRecentes = new List<AtividadeRecenteDTO>();
        
        // Últimas notas lançadas
        var notasRecentes = await _context.NotasAvaliacao
            .Include(n => n.Avaliacao)
                .ThenInclude(a => a.Disciplina)
            .Include(n => n.Aluno)
                .ThenInclude(a => a.Turma)
            .Where(n => turmaIds.Contains(n.Aluno.TurmaId))
            .OrderByDescending(n => n.DataLancamento)
            .Take(3)
            .Select(n => new AtividadeRecenteDTO
            {
                Acao = "Notas lançadas",
                TurmaNome = n.Aluno.Turma.Nome,
                DisciplinaNome = n.Avaliacao.Disciplina.Nome,
                DataHora = n.DataLancamento
            })
            .ToListAsync();
        
        atividadesRecentes.AddRange(notasRecentes);
        
        // Últimas frequências registradas - buscar aulas únicas que têm frequências
        var aulasComFrequencia = await _context.Aulas
            .Include(a => a.Turma)
                .ThenInclude(t => t.Disciplina)
            .Include(a => a.Frequencias)
            .Where(a => turmaIds.Contains(a.TurmaId) && a.Frequencias.Any())
            .OrderByDescending(a => a.Data)
            .Take(3)
            .ToListAsync();
        
        var frequenciasRecentesDTO = aulasComFrequencia.Select(a => new AtividadeRecenteDTO
        {
            Acao = "Frequência registrada",
            TurmaNome = a.Turma.Nome,
            DisciplinaNome = a.Turma.Disciplina.Nome,
            DataHora = a.Data.ToDateTime(TimeOnly.MinValue)
        }).ToList();
        
        atividadesRecentes.AddRange(frequenciasRecentesDTO);
        
        // Ordenar atividades recentes por data e pegar as 3 mais recentes
        atividadesRecentes = atividadesRecentes
            .OrderByDescending(a => a.DataHora)
            .Take(3)
            .ToList();
        
        return new DashboardDTO
        {
            TurmasAtivas = turmasAtivas,
            TotalAlunos = totalAlunos,
            Planejamentos = planejamentos,
            Avaliacoes = avaliacoes,
            ProximasAulas = proximasAulas,
            AtividadesRecentes = atividadesRecentes
        };
    }
}

