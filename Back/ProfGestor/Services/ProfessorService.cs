using AutoMapper;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Repositories;
using BCrypt.Net;

namespace ProfGestor.Services;

public class ProfessorService : IProfessorService
{
    private readonly IProfessorRepository _repository;
    private readonly IMapper _mapper;

    public ProfessorService(IProfessorRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProfessorDTO>> GetAllAsync()
    {
        var professores = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProfessorDTO>>(professores);
    }

    public async Task<ProfessorDTO?> GetByIdAsync(long id)
    {
        var professor = await _repository.GetByIdWithTurmasAsync(id);
        if (professor == null)
            return null;

        return _mapper.Map<ProfessorDTO>(professor);
    }

    public async Task<ProfessorDTO?> GetByEmailAsync(string email)
    {
        var professor = await _repository.GetByEmailAsync(email);
        if (professor == null)
            return null;

        return _mapper.Map<ProfessorDTO>(professor);
    }

    public async Task<ProfessorDTO> CreateAsync(ProfessorCreateDTO dto)
    {
        // Verificar se email já existe
        var emailExists = await _repository.ExistsAsync(p => p.Email == dto.Email);
        if (emailExists)
            throw new BusinessException("Email já cadastrado");

        var professor = _mapper.Map<Models.Professor>(dto);
        professor.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
        professor.UltimoLogin = null;

        var created = await _repository.AddAsync(professor);
        return _mapper.Map<ProfessorDTO>(created);
    }

    public async Task<ProfessorDTO> UpdateAsync(long id, ProfessorUpdateDTO dto)
    {
        var professor = await _repository.GetByIdAsync(id);
        if (professor == null)
            throw new NotFoundException("Professor", id);

        // Verificar se email já existe em outro professor
        var emailExists = await _repository.ExistsAsync(p => p.Email == dto.Email && p.Id != id);
        if (emailExists)
            throw new BusinessException("Email já cadastrado para outro professor");

        _mapper.Map(dto, professor);
        await _repository.UpdateAsync(professor);

        return _mapper.Map<ProfessorDTO>(professor);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var professor = await _repository.GetByIdAsync(id);
        if (professor == null)
            throw new NotFoundException("Professor", id);

        await _repository.DeleteAsync(professor);
        return true;
    }
}
