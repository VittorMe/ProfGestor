using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface IFrequenciaService
{
    Task<IEnumerable<FrequenciaDTO>> GetByAulaIdAsync(long aulaId);
    Task<FrequenciaDTO?> GetByIdAsync(long id);
    Task<FrequenciaDTO> CreateAsync(FrequenciaCreateDTO dto);
    Task<FrequenciaDTO> UpdateAsync(long id, FrequenciaUpdateDTO dto);
    Task<bool> DeleteAsync(long id);
    Task<AulaDTO> RegistrarFrequenciaAsync(RegistrarFrequenciaDTO dto);
}

