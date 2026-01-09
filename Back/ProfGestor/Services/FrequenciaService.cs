using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Models;
using ProfGestor.Models.Enums;
using ProfGestor.Repositories;

namespace ProfGestor.Services;

public class FrequenciaService : IFrequenciaService
{
    private readonly IFrequenciaRepository _repository;
    private readonly IAulaRepository _aulaRepository;
    private readonly IAlunoRepository _alunoRepository;
    private readonly ITurmaRepository _turmaRepository;
    private readonly ProfGestorContext _context;
    private readonly IMapper _mapper;

    public FrequenciaService(
        IFrequenciaRepository repository,
        IAulaRepository aulaRepository,
        IAlunoRepository alunoRepository,
        ITurmaRepository turmaRepository,
        ProfGestorContext context,
        IMapper mapper)
    {
        _repository = repository;
        _aulaRepository = aulaRepository;
        _alunoRepository = alunoRepository;
        _turmaRepository = turmaRepository;
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FrequenciaDTO>> GetByAulaIdAsync(long aulaId)
    {
        var frequencias = await _repository.GetByAulaIdWithDetailsAsync(aulaId);
        return _mapper.Map<IEnumerable<FrequenciaDTO>>(frequencias);
    }

    public async Task<FrequenciaDTO?> GetByIdAsync(long id)
    {
        var frequencia = await _repository.GetByIdAsync(id);
        if (frequencia == null)
            return null;

        var frequenciaCompleta = await _context.Frequencias
            .Include(f => f.Aluno)
            .Include(f => f.Aula)
            .FirstOrDefaultAsync(f => f.Id == id);

        return _mapper.Map<FrequenciaDTO>(frequenciaCompleta);
    }

    public async Task<FrequenciaDTO> CreateAsync(FrequenciaCreateDTO dto)
    {
        // Validar aula
        var aula = await _aulaRepository.GetByIdAsync(dto.AulaId);
        if (aula == null)
            throw new NotFoundException("Aula", dto.AulaId);

        // Validar aluno
        var aluno = await _alunoRepository.GetByIdAsync(dto.AlunoId);
        if (aluno == null)
            throw new NotFoundException("Aluno", dto.AlunoId);

        // Verificar se já existe frequência para este aluno nesta aula
        var frequenciaExistente = await _repository.GetByAulaIdAndAlunoIdAsync(dto.AulaId, dto.AlunoId);
        if (frequenciaExistente != null)
            throw new BusinessException("Frequência já registrada para este aluno nesta aula");

        var frequencia = _mapper.Map<Frequencia>(dto);
        var created = await _repository.AddAsync(frequencia);

        var createdCompleta = await _context.Frequencias
            .Include(f => f.Aluno)
            .Include(f => f.Aula)
            .FirstOrDefaultAsync(f => f.Id == created.Id);

        return _mapper.Map<FrequenciaDTO>(createdCompleta!);
    }

    public async Task<FrequenciaDTO> UpdateAsync(long id, FrequenciaUpdateDTO dto)
    {
        var frequencia = await _repository.GetByIdAsync(id);
        if (frequencia == null)
            throw new NotFoundException("Frequência", id);

        frequencia.Status = dto.Status;
        await _repository.UpdateAsync(frequencia);

        var updated = await _context.Frequencias
            .Include(f => f.Aluno)
            .Include(f => f.Aula)
            .FirstOrDefaultAsync(f => f.Id == id);

        return _mapper.Map<FrequenciaDTO>(updated!);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var frequencia = await _repository.GetByIdAsync(id);
        if (frequencia == null)
            throw new NotFoundException("Frequência", id);

        await _repository.DeleteAsync(frequencia);
        return true;
    }

    public async Task<AulaDTO> RegistrarFrequenciaAsync(RegistrarFrequenciaDTO dto)
    {
        // Usar transação para garantir atomicidade de todas as operações
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Validar turma
            var turma = await _turmaRepository.GetByIdWithDetailsAsync(dto.TurmaId);
            if (turma == null)
                throw new NotFoundException("Turma", dto.TurmaId);

            // Validar que todos os alunos pertencem à turma ANTES de fazer qualquer alteração
            var alunosIds = dto.Frequencias.Select(f => f.AlunoId).Distinct().ToList();
            var alunos = await _alunoRepository.GetByTurmaIdAsync(dto.TurmaId);
            var alunosIdsValidos = alunos.Select(a => a.Id).ToList();

            foreach (var alunoId in alunosIds)
            {
                if (!alunosIdsValidos.Contains(alunoId))
                    throw new BusinessException($"Aluno com ID {alunoId} não pertence à turma {dto.TurmaId}");
            }

            // Buscar ou criar aula
            var aulasExistentes = await _aulaRepository.GetByTurmaIdAndDataAsync(dto.TurmaId, dto.DataAula);
            var aulaExistente = aulasExistentes.FirstOrDefault();

            Models.Aula aula;
            if (aulaExistente != null)
            {
                aula = aulaExistente;
                // Atualizar período se necessário
                if (aula.Periodo != dto.Periodo)
                {
                    aula.Periodo = dto.Periodo;
                    _context.Aulas.Update(aula);
                }
            }
            else
            {
                // Criar nova aula
                aula = new Models.Aula
                {
                    Data = dto.DataAula,
                    Periodo = dto.Periodo,
                    TurmaId = dto.TurmaId
                };
                await _context.Aulas.AddAsync(aula);
            }

            // Salvar mudanças na aula antes de trabalhar com frequências
            await _context.SaveChangesAsync();

            // Remover frequências existentes para esta aula (usando RemoveRange para eficiência)
            var frequenciasExistentes = await _repository.GetByAulaIdAsync(aula.Id);
            if (frequenciasExistentes.Any())
            {
                _context.Frequencias.RemoveRange(frequenciasExistentes);
            }

            // Criar novas frequências
            var frequencias = dto.Frequencias.Select(f => new Frequencia
            {
                Status = f.Status,
                AlunoId = f.AlunoId,
                AulaId = aula.Id
            }).ToList();

            if (frequencias.Any())
            {
                await _context.Frequencias.AddRangeAsync(frequencias);
            }

            // Criar ou atualizar anotação da aula se fornecida
            if (!string.IsNullOrWhiteSpace(dto.AnotacaoTexto))
            {
                var anotacaoExistente = await _context.AnotacoesAula
                    .FirstOrDefaultAsync(a => a.AulaId == aula.Id);

                if (anotacaoExistente != null)
                {
                    anotacaoExistente.Texto = dto.AnotacaoTexto;
                    _context.AnotacoesAula.Update(anotacaoExistente);
                }
                else
                {
                    var novaAnotacao = new AnotacaoAula
                    {
                        Texto = dto.AnotacaoTexto,
                        AulaId = aula.Id,
                        DataRegistro = DateTime.Now
                    };
                    await _context.AnotacoesAula.AddAsync(novaAnotacao);
                }
            }

            // Salvar todas as mudanças de uma vez
            await _context.SaveChangesAsync();

            // Commit da transação
            await transaction.CommitAsync();

            // Retornar aula completa
            var aulaCompleta = await _aulaRepository.GetByIdWithDetailsAsync(aula.Id);
            return _mapper.Map<AulaDTO>(aulaCompleta!);
        }
        catch
        {
            // Rollback em caso de erro
            await transaction.RollbackAsync();
            throw;
        }
    }
}

