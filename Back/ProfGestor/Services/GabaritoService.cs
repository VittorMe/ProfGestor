using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Repositories;

namespace ProfGestor.Services;

public class GabaritoService : IGabaritoService
{
    private readonly IGabaritoQuestaoRepository _gabaritoRepository;
    private readonly IAvaliacaoRepository _avaliacaoRepository;
    private readonly ITurmaRepository _turmaRepository;
    private readonly ProfGestorContext _context;

    public GabaritoService(
        IGabaritoQuestaoRepository gabaritoRepository,
        IAvaliacaoRepository avaliacaoRepository,
        ITurmaRepository turmaRepository,
        ProfGestorContext context)
    {
        _gabaritoRepository = gabaritoRepository;
        _avaliacaoRepository = avaliacaoRepository;
        _turmaRepository = turmaRepository;
        _context = context;
    }

    public async Task<GabaritoResumoDTO> GetGabaritoResumoAsync(long avaliacaoId, long professorId)
    {
        // Buscar avaliação com questões
        var avaliacao = await _avaliacaoRepository.GetByIdWithDetailsAsync(avaliacaoId);
        if (avaliacao == null)
            throw new NotFoundException("Avaliacao", avaliacaoId);

        // Verificar se a disciplina da avaliação pertence ao professor
        var turmas = await _turmaRepository.GetByProfessorIdAsync(professorId);
        var disciplinaIds = turmas.Select(t => t.DisciplinaId).Distinct().ToList();

        if (!disciplinaIds.Contains(avaliacao.DisciplinaId))
            throw new UnauthorizedAccessException("Avaliação não pertence ao professor");

        // Buscar turma (primeira turma com essa disciplina)
        var turmasComDisciplina = turmas.Where(t => t.DisciplinaId == avaliacao.DisciplinaId).ToList();
        var turma = turmasComDisciplina.FirstOrDefault();
        if (turma == null)
            throw new NotFoundException("Nenhuma turma encontrada para esta disciplina");

        // Buscar questões objetivas ordenadas por número
        var questoes = await _context.QuestoesObjetivas
            .Include(q => q.GabaritoQuestao)
            .Where(q => q.AvaliacaoId == avaliacaoId)
            .OrderBy(q => q.Numero)
            .ToListAsync();

        // Buscar gabaritos existentes
        var gabaritosExistentes = await _gabaritoRepository.GetByAvaliacaoIdAsync(avaliacaoId);
        var gabaritosDict = gabaritosExistentes.ToDictionary(g => g.QuestaoObjetivaId, g => g);

        // Montar lista de questões com gabarito
        var questoesDTO = questoes.Select(questao =>
        {
            var temGabarito = gabaritosDict.TryGetValue(questao.Id, out var gabarito);
            
            return new QuestaoGabaritoDTO
            {
                Id = questao.Id,
                Numero = questao.Numero,
                Enunciado = questao.Enunciado,
                Valor = questao.Valor,
                AlternativaCorreta = temGabarito ? gabarito!.AlternativaCorreta : null,
                TemGabarito = temGabarito
            };
        }).ToList();

        return new GabaritoResumoDTO
        {
            AvaliacaoId = avaliacao.Id,
            AvaliacaoTitulo = avaliacao.Titulo,
            DisciplinaNome = avaliacao.Disciplina.Nome,
            TurmaNome = turma.Nome,
            Questoes = questoesDTO
        };
    }

    public async Task DefinirGabaritoAsync(DefinirGabaritoDTO dto, long professorId)
    {
        // Validar avaliação
        var avaliacao = await _avaliacaoRepository.GetByIdAsync(dto.AvaliacaoId);
        if (avaliacao == null)
            throw new NotFoundException("Avaliacao", dto.AvaliacaoId);

        // Verificar se a disciplina da avaliação pertence ao professor
        var turmas = await _turmaRepository.GetByProfessorIdAsync(professorId);
        var disciplinaIds = turmas.Select(t => t.DisciplinaId).Distinct().ToList();

        if (!disciplinaIds.Contains(avaliacao.DisciplinaId))
            throw new UnauthorizedAccessException("Avaliação não pertence ao professor");

        // Validar alternativas (A, B, C, D, E)
        var alternativasValidas = new[] { 'A', 'B', 'C', 'D', 'E' };
        
        foreach (var item in dto.Itens)
        {
            if (item.AlternativaCorreta.HasValue)
            {
                var alternativa = char.ToUpper(item.AlternativaCorreta.Value);
                if (!alternativasValidas.Contains(alternativa))
                    throw new BadRequestException($"Alternativa inválida: {item.AlternativaCorreta}. Use A, B, C, D ou E.");

                // Validar que a questão pertence à avaliação
                var questao = await _context.QuestoesObjetivas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(q => q.Id == item.QuestaoObjetivaId && q.AvaliacaoId == dto.AvaliacaoId);
                
                if (questao == null)
                    throw new NotFoundException("QuestaoObjetiva", item.QuestaoObjetivaId);
            }
            else
            {
                // Se não tem alternativa definida, verificar se existe gabarito para remover
                var gabaritoExistente = await _context.GabaritosQuestao
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.QuestaoObjetivaId == item.QuestaoObjetivaId);
                
                if (gabaritoExistente != null)
                {
                    // Remover gabarito existente se não há alternativa definida
                    var gabaritoParaRemover = await _context.GabaritosQuestao
                        .FirstOrDefaultAsync(g => g.Id == gabaritoExistente.Id);
                    
                    if (gabaritoParaRemover != null)
                    {
                        _context.GabaritosQuestao.Remove(gabaritoParaRemover);
                    }
                }
            }
        }

        // Usar transação para garantir atomicidade
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Limpar ChangeTracker para evitar problemas com entidades rastreadas
            var problematicEntries = _context.ChangeTracker.Entries()
                .Where(e => e.State != Microsoft.EntityFrameworkCore.EntityState.Detached &&
                            (e.Entity is Models.Aluno || e.Entity is Models.Frequencia || e.Entity is Models.Turma))
                .ToList();
            
            foreach (var entry in problematicEntries)
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            // Processar cada item do gabarito
            foreach (var item in dto.Itens)
            {
                if (!item.AlternativaCorreta.HasValue)
                    continue; // Pular itens sem alternativa definida

                var alternativa = char.ToUpper(item.AlternativaCorreta.Value);

                // Verificar se já existe gabarito para esta questão (usando AsNoTracking)
                var gabaritoExistente = await _context.GabaritosQuestao
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.QuestaoObjetivaId == item.QuestaoObjetivaId);

                if (gabaritoExistente != null)
                {
                    // Buscar a entidade para atualizar
                    var gabaritoParaAtualizar = await _context.GabaritosQuestao
                        .FirstOrDefaultAsync(g => g.Id == gabaritoExistente.Id);
                    
                    if (gabaritoParaAtualizar != null)
                    {
                        gabaritoParaAtualizar.AlternativaCorreta = alternativa;
                        // Não precisa setar o estado, o EF já está rastreando
                    }
                }
                else
                {
                    // Criar novo gabarito
                    var novoGabarito = new Models.GabaritoQuestao
                    {
                        QuestaoObjetivaId = item.QuestaoObjetivaId,
                        AlternativaCorreta = alternativa
                    };
                    await _context.GabaritosQuestao.AddAsync(novoGabarito);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Erro ao salvar gabarito: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
            }
            throw new BadRequestException($"Erro ao salvar gabarito: {ex.Message}");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Erro inesperado ao salvar gabarito: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            throw;
        }
    }
}

