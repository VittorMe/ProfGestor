using Moq;
using AutoMapper;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Models;
using ProfGestor.Repositories;
using ProfGestor.Services;
using ProfGestor.Mappings;

namespace TestProfGestor.Services;

public class ProfessorServiceTests
{
    private readonly Mock<IProfessorRepository> _mockRepository;
    private readonly IMapper _mapper;
    private readonly ProfessorService _service;

    public ProfessorServiceTests()
    {
        _mockRepository = new Mock<IProfessorRepository>();
        
        // Configurar AutoMapper
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        
        _service = new ProfessorService(_mockRepository.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProfessors()
    {
        // Arrange
        var professores = new List<Professor>
        {
            new Professor { Id = 1, Nome = "Professor 1", Email = "prof1@example.com" },
            new Professor { Id = 2, Nome = "Professor 2", Email = "prof2@example.com" }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(professores);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProfessor()
    {
        // Arrange
        var id = 1L;
        var professor = new Professor
        {
            Id = id,
            Nome = "Test Professor",
            Email = "test@example.com"
        };

        _mockRepository.Setup(r => r.GetByIdWithTurmasAsync(id)).ReturnsAsync(professor);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(professor.Nome, result.Nome);
        Assert.Equal(professor.Email, result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var id = 999L;
        _mockRepository.Setup(r => r.GetByIdWithTurmasAsync(id)).ReturnsAsync((Professor?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ReturnsProfessor()
    {
        // Arrange
        var email = "test@example.com";
        var professor = new Professor
        {
            Id = 1,
            Nome = "Test Professor",
            Email = email
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(professor);

        // Act
        var result = await _service.GetByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task CreateAsync_WithNewEmail_CreatesProfessor()
    {
        // Arrange
        var dto = new ProfessorCreateDTO
        {
            Nome = "New Professor",
            Email = "new@example.com",
            Senha = "senha123"
        };

        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Professor, bool>>>()))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Professor>()))
            .ReturnsAsync((Professor p) => { p.Id = 1; return p; });

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Nome, result.Nome);
        Assert.Equal(dto.Email, result.Email);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Professor>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithExistingEmail_ThrowsBusinessException()
    {
        // Arrange
        var dto = new ProfessorCreateDTO
        {
            Nome = "New Professor",
            Email = "existing@example.com",
            Senha = "senha123"
        };

        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Professor, bool>>>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _service.CreateAsync(dto));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Professor>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesProfessor()
    {
        // Arrange
        var id = 1L;
        var professor = new Professor
        {
            Id = id,
            Nome = "Old Name",
            Email = "old@example.com"
        };

        var dto = new ProfessorUpdateDTO
        {
            Nome = "New Name",
            Email = "new@example.com"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(professor);
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Professor, bool>>>()))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Professor>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(id, dto);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Professor>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsNotFoundException()
    {
        // Arrange
        var id = 999L;
        var dto = new ProfessorUpdateDTO { Nome = "Test", Email = "test@example.com" };

        _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Professor?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(id, dto));
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesProfessor()
    {
        // Arrange
        var id = 1L;
        var professor = new Professor { Id = id, Nome = "Test", Email = "test@example.com" };

        _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(professor);
        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Professor>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Professor>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ThrowsNotFoundException()
    {
        // Arrange
        var id = 999L;
        _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Professor?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(id));
    }
}
