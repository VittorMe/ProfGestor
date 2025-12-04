using AutoMapper;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Repositories;

namespace ProfGestor.Services;

public class AlunoService : IAlunoService
{
    private readonly IAlunoRepository _repository;
    private readonly ITurmaRepository _turmaRepository;
    private readonly IMapper _mapper;

    public AlunoService(IAlunoRepository repository, ITurmaRepository turmaRepository, IMapper mapper)
    {
        _repository = repository;
        _turmaRepository = turmaRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AlunoDTO>> GetAllAsync()
    {
        var alunos = await _repository.GetAllWithTurmaAsync();
        return _mapper.Map<IEnumerable<AlunoDTO>>(alunos);
    }

    public async Task<AlunoDTO?> GetByIdAsync(long id)
    {
        var aluno = await _repository.GetByIdWithDetailsAsync(id);
        if (aluno == null)
            return null;

        return _mapper.Map<AlunoDTO>(aluno);
    }

    public async Task<IEnumerable<AlunoDTO>> GetByTurmaIdAsync(long turmaId)
    {
        var turma = await _turmaRepository.GetByIdAsync(turmaId);
        if (turma == null)
            throw new NotFoundException("Turma", turmaId);

        var alunos = await _repository.GetByTurmaIdAsync(turmaId);
        return _mapper.Map<IEnumerable<AlunoDTO>>(alunos);
    }

    public async Task<AlunoDTO?> GetByMatriculaAsync(string matricula)
    {
        var aluno = await _repository.GetByMatriculaAsync(matricula);
        if (aluno == null)
            return null;

        return _mapper.Map<AlunoDTO>(aluno);
    }

    public async Task<AlunoDTO> CreateAsync(AlunoCreateDTO dto)
    {
        // Validar turma
        var turma = await _turmaRepository.GetByIdAsync(dto.TurmaId);
        if (turma == null)
            throw new NotFoundException("Turma", dto.TurmaId);

        // Verificar se matrícula já existe
        var matriculaExists = await _repository.ExistsAsync(a => a.Matricula == dto.Matricula);
        if (matriculaExists)
            throw new BusinessException("Matrícula já cadastrada");

        var aluno = _mapper.Map<Models.Aluno>(dto);
        var created = await _repository.AddAsync(aluno);
        return _mapper.Map<AlunoDTO>(created);
    }

    public async Task<AlunoDTO> UpdateAsync(long id, AlunoUpdateDTO dto)
    {
        var aluno = await _repository.GetByIdAsync(id);
        if (aluno == null)
            throw new NotFoundException("Aluno", id);

        // Validar turma
        var turma = await _turmaRepository.GetByIdAsync(dto.TurmaId);
        if (turma == null)
            throw new NotFoundException("Turma", dto.TurmaId);

        // Verificar se matrícula já existe em outro aluno
        var matriculaExists = await _repository.ExistsAsync(a => a.Matricula == dto.Matricula && a.Id != id);
        if (matriculaExists)
            throw new BusinessException("Matrícula já cadastrada para outro aluno");

        _mapper.Map(dto, aluno);
        await _repository.UpdateAsync(aluno);

        var updated = await _repository.GetByIdWithDetailsAsync(id);
        return _mapper.Map<AlunoDTO>(updated!);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var aluno = await _repository.GetByIdAsync(id);
        if (aluno == null)
            throw new NotFoundException("Aluno", id);

        await _repository.DeleteAsync(aluno);
        return true;
    }
}
