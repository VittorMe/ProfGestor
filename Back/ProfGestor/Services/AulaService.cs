using AutoMapper;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Repositories;

namespace ProfGestor.Services;

public class AulaService : IAulaService
{
    private readonly IAulaRepository _repository;
    private readonly ITurmaRepository _turmaRepository;
    private readonly IMapper _mapper;

    public AulaService(IAulaRepository repository, ITurmaRepository turmaRepository, IMapper mapper)
    {
        _repository = repository;
        _turmaRepository = turmaRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AulaDTO>> GetAllAsync()
    {
        var aulas = await _repository.GetAllWithDetailsAsync();
        return _mapper.Map<IEnumerable<AulaDTO>>(aulas);
    }

    public async Task<AulaDTO?> GetByIdAsync(long id)
    {
        var aula = await _repository.GetByIdWithDetailsAsync(id);
        if (aula == null)
            return null;

        return _mapper.Map<AulaDTO>(aula);
    }

    public async Task<IEnumerable<AulaDTO>> GetByTurmaIdAsync(long turmaId)
    {
        var turma = await _turmaRepository.GetByIdAsync(turmaId);
        if (turma == null)
            throw new NotFoundException("Turma", turmaId);

        var aulas = await _repository.GetByTurmaIdAsync(turmaId);
        return _mapper.Map<IEnumerable<AulaDTO>>(aulas);
    }

    public async Task<AulaDTO?> GetByTurmaIdAndDataAsync(long turmaId, DateOnly data)
    {
        var turma = await _turmaRepository.GetByIdAsync(turmaId);
        if (turma == null)
            throw new NotFoundException("Turma", turmaId);

        var aulas = await _repository.GetByTurmaIdAndDataAsync(turmaId, data);
        var aula = aulas.FirstOrDefault();
        if (aula == null)
            return null;

        var aulaCompleta = await _repository.GetByIdWithDetailsAsync(aula.Id);
        return _mapper.Map<AulaDTO>(aulaCompleta);
    }

    public async Task<AulaDTO> CreateAsync(AulaCreateDTO dto)
    {
        var turma = await _turmaRepository.GetByIdAsync(dto.TurmaId);
        if (turma == null)
            throw new NotFoundException("Turma", dto.TurmaId);

        var aula = _mapper.Map<Models.Aula>(dto);
        var created = await _repository.AddAsync(aula);
        
        var createdWithDetails = await _repository.GetByIdWithDetailsAsync(created.Id);
        return _mapper.Map<AulaDTO>(createdWithDetails!);
    }

    public async Task<AulaDTO> UpdateAsync(long id, AulaUpdateDTO dto)
    {
        var aula = await _repository.GetByIdAsync(id);
        if (aula == null)
            throw new NotFoundException("Aula", id);

        _mapper.Map(dto, aula);
        await _repository.UpdateAsync(aula);

        var updated = await _repository.GetByIdWithDetailsAsync(id);
        return _mapper.Map<AulaDTO>(updated!);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var aula = await _repository.GetByIdAsync(id);
        if (aula == null)
            throw new NotFoundException("Aula", id);

        await _repository.DeleteAsync(aula);
        return true;
    }
}


