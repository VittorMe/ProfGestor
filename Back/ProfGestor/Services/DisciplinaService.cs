using AutoMapper;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Repositories;

namespace ProfGestor.Services;

public class DisciplinaService : IDisciplinaService
{
    private readonly IRepository<Models.Disciplina> _repository;
    private readonly IMapper _mapper;

    public DisciplinaService(IRepository<Models.Disciplina> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DisciplinaDTO>> GetAllAsync()
    {
        var disciplinas = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<DisciplinaDTO>>(disciplinas);
    }

    public async Task<DisciplinaDTO?> GetByIdAsync(long id)
    {
        var disciplina = await _repository.GetByIdAsync(id);
        if (disciplina == null)
            return null;

        return _mapper.Map<DisciplinaDTO>(disciplina);
    }

    public async Task<DisciplinaDTO> CreateAsync(DisciplinaCreateDTO dto)
    {
        // Verificar se disciplina com mesmo nome já existe
        var nomeExists = await _repository.ExistsAsync(d => d.Nome.ToLower() == dto.Nome.ToLower());
        if (nomeExists)
            throw new BusinessException("Disciplina com este nome já existe");

        var disciplina = _mapper.Map<Models.Disciplina>(dto);
        var created = await _repository.AddAsync(disciplina);
        return _mapper.Map<DisciplinaDTO>(created);
    }

    public async Task<DisciplinaDTO> UpdateAsync(long id, DisciplinaUpdateDTO dto)
    {
        var disciplina = await _repository.GetByIdAsync(id);
        if (disciplina == null)
            throw new NotFoundException("Disciplina", id);

        // Verificar se nome já existe em outra disciplina
        var nomeExists = await _repository.ExistsAsync(d => d.Nome.ToLower() == dto.Nome.ToLower() && d.Id != id);
        if (nomeExists)
            throw new BusinessException("Disciplina com este nome já existe");

        _mapper.Map(dto, disciplina);
        await _repository.UpdateAsync(disciplina);

        return _mapper.Map<DisciplinaDTO>(disciplina);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var disciplina = await _repository.GetByIdAsync(id);
        if (disciplina == null)
            throw new NotFoundException("Disciplina", id);

        await _repository.DeleteAsync(disciplina);
        return true;
    }
}


