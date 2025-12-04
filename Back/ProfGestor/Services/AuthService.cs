using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProfGestor.DTOs;
using ProfGestor.Repositories;
using BCrypt.Net;

namespace ProfGestor.Services;

public class AuthService : IAuthService
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IProfessorRepository professorRepository, IConfiguration configuration)
    {
        _professorRepository = professorRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var professor = await _professorRepository.GetByEmailAsync(request.Email);

        if (professor == null)
            return null;

        // Verificar senha
        if (!BCrypt.Net.BCrypt.Verify(request.Senha, professor.SenhaHash))
            return null;

        // Atualizar último login
        professor.UltimoLogin = DateTime.Now;
        await _professorRepository.UpdateAsync(professor);

        // Gerar token
        var token = GenerateToken(professor);

        return new LoginResponse
        {
            Token = token,
            ExpiraEm = DateTime.Now.AddHours(8), // Token expira em 8 horas
            Professor = new ProfessorInfo
            {
                Id = professor.Id,
                Nome = professor.Nome,
                Email = professor.Email
            }
        };
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        // Verificar se email já existe
        var emailExists = await _professorRepository.ExistsAsync(p => p.Email == request.Email);

        if (emailExists)
            return false;

        // Criar novo professor
        var professor = new Models.Professor
        {
            Nome = request.Nome,
            Email = request.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            UltimoLogin = null
        };

        await _professorRepository.AddAsync(professor);

        return true;
    }

    public async Task<bool> ChangePasswordAsync(long professorId, string senhaAtual, string novaSenha)
    {
        var professor = await _professorRepository.GetByIdAsync(professorId);
        if (professor == null)
            return false;

        // Verificar senha atual
        if (!BCrypt.Net.BCrypt.Verify(senhaAtual, professor.SenhaHash))
            return false;

        // Atualizar senha
        professor.SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
        await _professorRepository.UpdateAsync(professor);

        return true;
    }

    private string GenerateToken(Models.Professor professor)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "MinhaChaveSecretaSuperSeguraParaJWT2024!@#$%";
        var issuer = jwtSettings["Issuer"] ?? "ProfGestor";
        var audience = jwtSettings["Audience"] ?? "ProfGestorUsers";
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "480"); // 8 horas

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, professor.Id.ToString()),
            new Claim(ClaimTypes.Name, professor.Nome),
            new Claim(ClaimTypes.Email, professor.Email),
            new Claim("ProfessorId", professor.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
