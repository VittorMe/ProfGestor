using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface INotaAvaliacaoService
{
    Task<LancamentoNotasResumoDTO> GetLancamentoNotasAsync(long avaliacaoId, long professorId);
    Task LancarNotasAsync(LancarNotasDTO dto, long professorId);
    Task<NotaAvaliacaoDTO?> GetByIdAsync(long id);
    Task<NotaAvaliacaoDTO> CreateAsync(NotaAvaliacaoCreateDTO dto);
    Task<NotaAvaliacaoDTO> UpdateAsync(long id, NotaAvaliacaoUpdateDTO dto);
    Task<bool> DeleteAsync(long id);
}

