using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface IRelatorioService
{
    Task<RelatorioFrequenciaDTO> GerarRelatorioFrequenciaAsync(RelatorioFrequenciaRequestDTO request);
    Task<RelatorioDesempenhoDTO> GerarRelatorioDesempenhoAsync(RelatorioDesempenhoRequestDTO request);
}
