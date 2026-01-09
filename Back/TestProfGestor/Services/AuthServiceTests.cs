using Moq;
using ProfGestor.DTOs;
using ProfGestor.Models;
using ProfGestor.Repositories;
using ProfGestor.Services;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;

namespace TestProfGestor.Services;

public class AuthServiceTests
{
    private readonly Mock<IProfessorRepository> _mockRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockRepository = new Mock<IProfessorRepository>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        // Configurar mock de configuração
        var jwtSection = new Mock<IConfigurationSection>();
        jwtSection.Setup(x => x["SecretKey"]).Returns("TestSecretKey123456789012345678901234567890");
        jwtSection.Setup(x => x["Issuer"]).Returns("TestIssuer");
        jwtSection.Setup(x => x["Audience"]).Returns("TestAudience");
        jwtSection.Setup(x => x["ExpirationMinutes"]).Returns("480");
        
        _mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSection.Object);
        
        _authService = new AuthService(_mockRepository.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
    {
        // Arrange
        var email = "test@example.com";
        var senha = "senha123";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
        
        var professor = new Professor
        {
            Id = 1,
            Nome = "Test Professor",
            Email = email,
            SenhaHash = senhaHash,
            UltimoLogin = null
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(professor);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Professor>())).Returns(Task.CompletedTask);

        var request = new LoginRequest { Email = email, Senha = senha };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.NotNull(result.Professor);
        Assert.Equal(professor.Id, result.Professor.Id);
        Assert.Equal(professor.Nome, result.Professor.Nome);
        Assert.Equal(professor.Email, result.Professor.Email);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Professor>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ReturnsNull()
    {
        // Arrange
        var request = new LoginRequest { Email = "invalid@example.com", Senha = "senha123" };
        _mockRepository.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((Professor?)null);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        var email = "test@example.com";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("correctPassword");
        
        var professor = new Professor
        {
            Id = 1,
            Nome = "Test Professor",
            Email = email,
            SenhaHash = senhaHash
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(professor);

        var request = new LoginRequest { Email = email, Senha = "wrongPassword" };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Professor>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WithNewEmail_ReturnsTrue()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Nome = "New Professor",
            Email = "new@example.com",
            Senha = "senha123"
        };

        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Professor, bool>>>()))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Professor>()))
            .ReturnsAsync((Professor p) => p);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Professor>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ReturnsFalse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Nome = "New Professor",
            Email = "existing@example.com",
            Senha = "senha123"
        };

        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Professor, bool>>>()))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Professor>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithValidCurrentPassword_ReturnsTrue()
    {
        // Arrange
        var professorId = 1L;
        var senhaAtual = "oldPassword";
        var novaSenha = "newPassword";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(senhaAtual);

        var professor = new Professor
        {
            Id = professorId,
            Nome = "Test Professor",
            Email = "test@example.com",
            SenhaHash = senhaHash
        };

        _mockRepository.Setup(r => r.GetByIdAsync(professorId)).ReturnsAsync(professor);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Professor>())).Returns(Task.CompletedTask);

        // Act
        var result = await _authService.ChangePasswordAsync(professorId, senhaAtual, novaSenha);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Professor>()), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithInvalidCurrentPassword_ReturnsFalse()
    {
        // Arrange
        var professorId = 1L;
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("correctPassword");

        var professor = new Professor
        {
            Id = professorId,
            Nome = "Test Professor",
            Email = "test@example.com",
            SenhaHash = senhaHash
        };

        _mockRepository.Setup(r => r.GetByIdAsync(professorId)).ReturnsAsync(professor);

        // Act
        var result = await _authService.ChangePasswordAsync(professorId, "wrongPassword", "newPassword");

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Professor>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithNonExistentProfessor_ReturnsFalse()
    {
        // Arrange
        var professorId = 999L;
        _mockRepository.Setup(r => r.GetByIdAsync(professorId)).ReturnsAsync((Professor?)null);

        // Act
        var result = await _authService.ChangePasswordAsync(professorId, "oldPassword", "newPassword");

        // Assert
        Assert.False(result);
    }
}
