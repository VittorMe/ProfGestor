using ProfGestor.DTOs;

namespace ProfGestor.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<bool> ChangePasswordAsync(long professorId, string senhaAtual, string novaSenha);
}

