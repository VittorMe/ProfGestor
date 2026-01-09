using Moq;
using AutoMapper;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Models;
using ProfGestor.Repositories;
using ProfGestor.Services;
using ProfGestor.Mappings;

namespace TestProfGestor.Services;

public class DisciplinaServiceTests
{
    private readonly Mock<IRepository<Disciplina>> _mockRepository;
    private readonly IMapper _mapper;
    private readonly DisciplinaService _service;

    public DisciplinaServiceTests()
    {
        _mockRepository = new Mock<IRepository<Disciplina>>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        
        _service = new DisciplinaService(_mockRepository.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllDisciplinas()
    {
        // Arrange
        var disciplinas = new List<Disciplina>
        {
            new Disciplina { Id = 1, Nome = "Matemática" },
            new Disciplina { Id = 2, Nome = "Português" }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(disciplinas);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsDisciplina()
    {
        // Arrange
        var id = 1L;
        var disciplina = new Disciplina { Id = id, Nome = "Matemática" };

        _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(disciplina);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(disciplina.Nome, result.Nome);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var id = 999L;
        _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Disciplina?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WithNewNome_CreatesDisciplina()
    {
        // Arrange
        var dto = new DisciplinaCreateDTO { Nome = "Física" };

        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Disciplina, bool>>>()))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Disciplina>()))
            .ReturnsAsync((Disciplina d) => { d.Id = 1; return d; });

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Nome, result.Nome);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Disciplina>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithExistingNome_ThrowsBusinessException()
    {
        // Arrange
        var dto = new DisciplinaCreateDTO { Nome = "Matemática" };

        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Disciplina, bool>>>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _service.CreateAsync(dto));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Disciplina>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesDisciplina()
    {
        // Arrange
        var id = 1L;
        var disciplina = new Disciplina { Id = id, Nome = "Matemática" };
        var dto = new DisciplinaUpdateDTO { Nome = "Matemática Avançada" };

        _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(disciplina);
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Disciplina, bool>>>()))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Disciplina>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(id, dto);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Disciplina>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsNotFoundException()
    {
        // Arrange
        var id = 999L;
        var dto = new DisciplinaUpdateDTO { Nome = "Test" };

        _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Disciplina?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(id, dto));
    }
}
