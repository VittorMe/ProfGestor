using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface IDashboardService
{
    Task<DashboardDTO> GetDashboardDataAsync(long professorId);
}


