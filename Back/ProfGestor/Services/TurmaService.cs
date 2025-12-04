using AutoMapper;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Repositories;

namespace ProfGestor.Services;

public class TurmaService : ITurmaService
{
    private readonly ITurmaRepository _repository;
    private readonly IProfessorRepository _professorRepository;
    private readonly IRepository<Models.Disciplina> _disciplinaRepository;
    private readonly IMapper _mapper;

    public TurmaService(
        ITurmaRepository repository,
        IProfessorRepository professorRepository,
        IRepository<Models.Disciplina> disciplinaRepository,
        IMapper mapper)
    {
        _repository = repository;
        _professorRepository = professorRepository;
        _disciplinaRepository = disciplinaRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TurmaDTO>> GetAllAsync()
    {
        var turmas = await _repository.GetAllWithDetailsAsync();
        return _mapper.Map<IEnumerable<TurmaDTO>>(turmas);
    }

    public async Task<TurmaDTO?> GetByIdAsync(long id)
    {
        var turma = await _repository.GetByIdWithDetailsAsync(id);
        if (turma == null)
            return null;

        return _mapper.Map<TurmaDTO>(turma);
    }

    public async Task<IEnumerable<TurmaDTO>> GetByProfessorIdAsync(long professorId)
    {
        var professor = await _professorRepository.GetByIdAsync(professorId);
        if (professor == null)
            throw new NotFoundException("Professor", professorId);

        var turmas = await _repository.GetByProfessorIdAsync(professorId);
        return _mapper.Map<IEnumerable<TurmaDTO>>(turmas);
    }

    public async Task<TurmaDTO> CreateAsync(TurmaCreateDTO dto)
    {
        // Validar professor
        var professor = await _professorRepository.GetByIdAsync(dto.ProfessorId);
        if (professor == null)
            throw new NotFoundException("Professor", dto.ProfessorId);

        // Validar disciplina
        var disciplina = await _disciplinaRepository.GetByIdAsync(dto.DisciplinaId);
        if (disciplina == null)
            throw new NotFoundException("Disciplina", dto.DisciplinaId);

        var turma = _mapper.Map<Models.Turma>(dto);
        var created = await _repository.AddAsync(turma);
        return _mapper.Map<TurmaDTO>(created);
    }

    public async Task<TurmaDTO> UpdateAsync(long id, TurmaUpdateDTO dto)
    {
        var turma = await _repository.GetByIdAsync(id);
        if (turma == null)
            throw new NotFoundException("Turma", id);

        // Validar professor
        var professor = await _professorRepository.GetByIdAsync(dto.ProfessorId);
        if (professor == null)
            throw new NotFoundException("Professor", dto.ProfessorId);

        // Validar disciplina
        var disciplina = await _disciplinaRepository.GetByIdAsync(dto.DisciplinaId);
        if (disciplina == null)
            throw new NotFoundException("Disciplina", dto.DisciplinaId);

        _mapper.Map(dto, turma);
        await _repository.UpdateAsync(turma);

        var updated = await _repository.GetByIdWithDetailsAsync(id);
        return _mapper.Map<TurmaDTO>(updated!);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var turma = await _repository.GetByIdAsync(id);
        if (turma == null)
            throw new NotFoundException("Turma", id);

        await _repository.DeleteAsync(turma);
        return true;
    }
}
