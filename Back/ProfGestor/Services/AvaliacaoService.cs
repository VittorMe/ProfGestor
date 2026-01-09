using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Models.Enums;
using ProfGestor.Repositories;

namespace ProfGestor.Services;

public class AvaliacaoService : IAvaliacaoService
{
    private readonly IAvaliacaoRepository _repository;
    private readonly IRepository<Models.Disciplina> _disciplinaRepository;
    private readonly ProfGestorContext _context;
    private readonly IMapper _mapper;

    public AvaliacaoService(
        IAvaliacaoRepository repository,
        IRepository<Models.Disciplina> disciplinaRepository,
        ProfGestorContext context,
        IMapper mapper)
    {
        _repository = repository;
        _disciplinaRepository = disciplinaRepository;
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AvaliacaoDTO>> GetAllAsync()
    {
        var avaliacoes = await _repository.GetAllWithDetailsAsync();
        var dtos = _mapper.Map<IEnumerable<AvaliacaoDTO>>(avaliacoes);
        
        // Enriquecer DTOs com informações adicionais
        foreach (var dto in dtos)
        {
            await EnriquecerDTOAsync(dto);
        }
        
        return dtos;
    }

    public async Task<AvaliacaoDTO?> GetByIdAsync(long id)
    {
        var avaliacao = await _repository.GetByIdWithDetailsAsync(id);
        if (avaliacao == null)
            return null;

        var dto = _mapper.Map<AvaliacaoDTO>(avaliacao);
        await EnriquecerDTOAsync(dto);
        return dto;
    }

    public async Task<IEnumerable<AvaliacaoDTO>> GetByDisciplinaIdAsync(long disciplinaId)
    {
        var disciplina = await _disciplinaRepository.GetByIdAsync(disciplinaId);
        if (disciplina == null)
            throw new NotFoundException("Disciplina", disciplinaId);

        var avaliacoes = await _repository.GetByDisciplinaIdAsync(disciplinaId);
        var dtos = _mapper.Map<IEnumerable<AvaliacaoDTO>>(avaliacoes);
        
        // Enriquecer DTOs com informações adicionais
        foreach (var dto in dtos)
        {
            await EnriquecerDTOAsync(dto);
        }
        
        return dtos;
    }

    public async Task<AvaliacaoDTO> CreateAsync(AvaliacaoCreateDTO dto)
    {
        // Validar disciplina usando AsNoTracking para evitar problemas de rastreamento
        var disciplinaExiste = await _context.Disciplinas
            .AsNoTracking()
            .AnyAsync(d => d.Id == dto.DisciplinaId);

        if (!disciplinaExiste)
            throw new NotFoundException("Disciplina", dto.DisciplinaId);

        // Validar se já existe avaliação na mesma disciplina e data
        var avaliacaoExistente = await _repository.ExistsByDisciplinaIdAndDataAsync(dto.DisciplinaId, dto.DataAplicacao);
        if (avaliacaoExistente)
            throw new BadRequestException($"Já existe uma avaliação para esta disciplina na data {dto.DataAplicacao:dd/MM/yyyy}.");

        // IMPORTANTE: Detectar e remover entidades problemáticas do ChangeTracker
        // Isso evita que entidades Aluno/Frequencia carregadas anteriormente interfiram
        var problematicEntries = _context.ChangeTracker.Entries()
            .Where(e => e.State != EntityState.Detached &&
                        (e.Entity is Models.Aluno || e.Entity is Models.Frequencia || e.Entity is Models.Turma))
            .ToList();

        foreach (var entry in problematicEntries)
        {
            entry.State = EntityState.Detached;
        }

        // Usar transação para garantir atomicidade
        using var transaction = await _context.Database.BeginTransactionAsync();
        Models.Avaliacao avaliacao;
        try
        {
            // Criar a avaliação manualmente para garantir que apenas o DisciplinaId seja usado
            avaliacao = new Models.Avaliacao
            {
                Titulo = dto.Titulo,
                Tipo = dto.Tipo,
                DataAplicacao = dto.DataAplicacao,
                ValorMaximo = dto.ValorMaximo,
                DisciplinaId = dto.DisciplinaId
            };

            // Adicionar a entidade diretamente ao DbSet
            await _context.Avaliacoes.AddAsync(avaliacao);
            await _context.SaveChangesAsync();

            // Se for avaliação objetiva, criar questões
            if (dto.IsObjetiva && dto.QuestoesObjetivas != null && dto.QuestoesObjetivas.Any())
            {
                // Validar alternativas corretas
                var alternativasValidas = new[] { 'A', 'B', 'C', 'D', 'E' };
                
                foreach (var questaoDto in dto.QuestoesObjetivas)
                {
                    char? alternativaCorreta = null;
                    if (!string.IsNullOrWhiteSpace(questaoDto.AlternativaCorreta))
                    {
                        var alternativa = questaoDto.AlternativaCorreta.Trim().ToUpper()[0];
                        if (!alternativasValidas.Contains(alternativa))
                        {
                            throw new BadRequestException($"Alternativa correta inválida para questão {questaoDto.Numero}. Use A, B, C, D ou E.");
                        }
                        alternativaCorreta = alternativa;
                    }

                    // Criar questão objetiva
                    var questao = new Models.QuestaoObjetiva
                    {
                        Numero = questaoDto.Numero,
                        Enunciado = questaoDto.Enunciado,
                        Valor = questaoDto.Valor,
                        AvaliacaoId = avaliacao.Id
                    };

                    await _context.QuestoesObjetivas.AddAsync(questao);
                    await _context.SaveChangesAsync();

                    // Se houver alternativa correta, criar gabarito
                    if (alternativaCorreta.HasValue)
                    {
                        var gabarito = new Models.GabaritoQuestao
                        {
                            AlternativaCorreta = alternativaCorreta.Value,
                            QuestaoObjetivaId = questao.Id
                        };

                        await _context.GabaritosQuestao.AddAsync(gabarito);
                    }
                }

                await _context.SaveChangesAsync();
            }

            // Commit da transação
            await transaction.CommitAsync();
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            // Log detalhado do erro para debug
            Console.WriteLine($"Erro ao salvar Avaliacao: {ex.Message}");
            Console.WriteLine($"DisciplinaId: {dto.DisciplinaId}");
            Console.WriteLine($"Entidades rastreadas antes do detach: {problematicEntries.Count}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                Console.WriteLine($"InnerException StackTrace: {ex.InnerException.StackTrace}");
            }
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

        // Buscar a avaliação criada com todos os detalhes
        var createdCompleta = await _repository.GetByIdWithDetailsAsync(avaliacao.Id);
        if (createdCompleta == null)
            throw new NotFoundException("Avaliacao", avaliacao.Id);

        var dtoResult = _mapper.Map<AvaliacaoDTO>(createdCompleta);

        // Enriquecer DTO com informações adicionais
        try
        {
            await EnriquecerDTOAsync(dtoResult);
        }
        catch (Exception)
        {
            // Se houver erro ao enriquecer, ainda retornar o DTO básico
            dtoResult.TipoDisplay = dto.IsObjetiva ? "Objetiva" : "Subjetiva";
            dtoResult.TemGabarito = dto.IsObjetiva && dto.QuestoesObjetivas?.Any(q => !string.IsNullOrWhiteSpace(q.AlternativaCorreta)) == true;
            dtoResult.TotalQuestoes = dto.QuestoesObjetivas?.Count ?? 0;
            dtoResult.TemNotasLancadas = false;
        }

        return dtoResult;

        // Buscar a avaliação criada com todos os detalhes
        createdCompleta = await _repository.GetByIdWithDetailsAsync(avaliacao.Id);
        if (createdCompleta == null)
            throw new NotFoundException("Avaliacao", avaliacao.Id);

        dtoResult = _mapper.Map<AvaliacaoDTO>(createdCompleta);

        // Enriquecer DTO com informações adicionais
        try
        {
            await EnriquecerDTOAsync(dtoResult);
        }
        catch (Exception)
        {
            // Se houver erro ao enriquecer, ainda retornar o DTO básico
            dtoResult.TipoDisplay = "Subjetiva"; // Valor padrão
            dtoResult.TemGabarito = false;
            dtoResult.TotalQuestoes = 0;
            dtoResult.TemNotasLancadas = false;
        }

        return dtoResult;
    }
    public async Task<AvaliacaoDTO> UpdateAsync(long id, AvaliacaoUpdateDTO dto)
    {
        var avaliacao = await _repository.GetByIdAsync(id);
        if (avaliacao == null)
            throw new NotFoundException("Avaliacao", id);

        // Validar disciplina
        var disciplina = await _disciplinaRepository.GetByIdAsync(dto.DisciplinaId);
        if (disciplina == null)
            throw new NotFoundException("Disciplina", dto.DisciplinaId);

        // Validar se já existe outra avaliação na mesma disciplina e data (excluindo a atual)
        var avaliacaoExistente = await _repository.ExistsByDisciplinaIdAndDataAsync(dto.DisciplinaId, dto.DataAplicacao, id);
        if (avaliacaoExistente)
            throw new BadRequestException($"Já existe outra avaliação para esta disciplina na data {dto.DataAplicacao:dd/MM/yyyy}.");

        _mapper.Map(dto, avaliacao);
        await _repository.UpdateAsync(avaliacao);

        var updated = await _repository.GetByIdWithDetailsAsync(id);
        var dtoResult = _mapper.Map<AvaliacaoDTO>(updated!);
        await EnriquecerDTOAsync(dtoResult);
        
        return dtoResult;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var avaliacao = await _repository.GetByIdAsync(id);
        if (avaliacao == null)
            throw new NotFoundException("Avaliacao", id);

        await _repository.DeleteAsync(avaliacao);
        return true;
    }

    private async Task EnriquecerDTOAsync(AvaliacaoDTO dto)
    {
        // Verificar se tem gabarito (tem questões objetivas com gabarito)
        var avaliacao = await _context.Avaliacoes
            .Include(a => a.QuestoesObjetivas)
                .ThenInclude(q => q.GabaritoQuestao)
            .FirstOrDefaultAsync(a => a.Id == dto.Id);

        if (avaliacao != null)
        {
            dto.TemGabarito = avaliacao.QuestoesObjetivas.Any(q => q.GabaritoQuestao != null);
            dto.TotalQuestoes = avaliacao.QuestoesObjetivas.Count;
            dto.TemNotasLancadas = await _context.NotasAvaliacao.AnyAsync(n => n.AvaliacaoId == dto.Id);
            
            // Formatar tipo para exibição
            dto.TipoDisplay = FormatTipoAvaliacao(dto.Tipo, dto.TemGabarito, dto.TotalQuestoes);
        }
    }

    private string FormatTipoAvaliacao(TipoAvaliacao tipo, bool temGabarito, int totalQuestoes)
    {
        // Determinar tipo baseado apenas na existência de questões objetivas
        // Se tem questões objetivas, pode ser Objetiva ou Mista
        // Se não tem questões objetivas, é Subjetiva
        if (totalQuestoes > 0)
        {
            // Tem questões objetivas
            if (temGabarito)
            {
                // Se tem gabarito definido, é "Objetiva"
                return "Objetiva";
            }
            else
            {
                // Tem questões mas sem gabarito ainda, ainda é "Objetiva" (pode definir gabarito depois)
                return "Objetiva";
            }
        }
        else
        {
            // Sem questões objetivas, é subjetiva
            return "Subjetiva";
        }
    }
}

