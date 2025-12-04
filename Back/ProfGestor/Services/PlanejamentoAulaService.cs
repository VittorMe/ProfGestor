using AutoMapper;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Repositories;

namespace ProfGestor.Services;

public class PlanejamentoAulaService : IPlanejamentoAulaService
{
    private readonly IPlanejamentoAulaRepository _repository;
    private readonly IRepository<Models.Disciplina> _disciplinaRepository;
    private readonly IMapper _mapper;

    public PlanejamentoAulaService(
        IPlanejamentoAulaRepository repository,
        IRepository<Models.Disciplina> disciplinaRepository,
        IMapper mapper)
    {
        _repository = repository;
        _disciplinaRepository = disciplinaRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PlanejamentoAulaDTO>> GetAllAsync()
    {
        var planejamentos = await _repository.GetAllWithDetailsAsync();
        return _mapper.Map<IEnumerable<PlanejamentoAulaDTO>>(planejamentos);
    }

    public async Task<PlanejamentoAulaDTO?> GetByIdAsync(long id)
    {
        var planejamento = await _repository.GetByIdWithDetailsAsync(id);
        if (planejamento == null)
            return null;

        return _mapper.Map<PlanejamentoAulaDTO>(planejamento);
    }

    public async Task<IEnumerable<PlanejamentoAulaDTO>> GetByDisciplinaIdAsync(long disciplinaId)
    {
        var disciplina = await _disciplinaRepository.GetByIdAsync(disciplinaId);
        if (disciplina == null)
            throw new NotFoundException("Disciplina", disciplinaId);

        var planejamentos = await _repository.GetByDisciplinaIdAsync(disciplinaId);
        return _mapper.Map<IEnumerable<PlanejamentoAulaDTO>>(planejamentos);
    }

    public async Task<IEnumerable<PlanejamentoAulaDTO>> GetFavoritosAsync()
    {
        var planejamentos = await _repository.GetFavoritosAsync();
        return _mapper.Map<IEnumerable<PlanejamentoAulaDTO>>(planejamentos);
    }

    public async Task<PlanejamentoAulaDTO> CreateAsync(PlanejamentoAulaCreateDTO dto)
    {
        // Validar disciplina
        var disciplina = await _disciplinaRepository.GetByIdAsync(dto.DisciplinaId);
        if (disciplina == null)
            throw new NotFoundException("Disciplina", dto.DisciplinaId);

        var planejamento = _mapper.Map<Models.PlanejamentoAula>(dto);
        planejamento.CriadoEm = DateTime.Now;

        var created = await _repository.AddAsync(planejamento);
        return _mapper.Map<PlanejamentoAulaDTO>(created);
    }

    public async Task<PlanejamentoAulaDTO> UpdateAsync(long id, PlanejamentoAulaUpdateDTO dto)
    {
        var planejamento = await _repository.GetByIdAsync(id);
        if (planejamento == null)
            throw new NotFoundException("PlanejamentoAula", id);

        // Validar disciplina
        var disciplina = await _disciplinaRepository.GetByIdAsync(dto.DisciplinaId);
        if (disciplina == null)
            throw new NotFoundException("Disciplina", dto.DisciplinaId);

        _mapper.Map(dto, planejamento);
        await _repository.UpdateAsync(planejamento);

        var updated = await _repository.GetByIdWithDetailsAsync(id);
        return _mapper.Map<PlanejamentoAulaDTO>(updated!);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var planejamento = await _repository.GetByIdAsync(id);
        if (planejamento == null)
            throw new NotFoundException("PlanejamentoAula", id);

        await _repository.DeleteAsync(planejamento);
        return true;
    }
}
