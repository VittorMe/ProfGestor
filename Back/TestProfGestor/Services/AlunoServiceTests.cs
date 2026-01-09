using Moq;
using AutoMapper;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Models;
using ProfGestor.Repositories;
using ProfGestor.Services;
using ProfGestor.Mappings;

namespace TestProfGestor.Services;

public class AlunoServiceTests
{
    private readonly Mock<IAlunoRepository> _mockRepository;
    private readonly Mock<ITurmaRepository> _mockTurmaRepository;
    private readonly IMapper _mapper;
    private readonly AlunoService _service;

    public AlunoServiceTests()
    {
        _mockRepository = new Mock<IAlunoRepository>();
        _mockTurmaRepository = new Mock<ITurmaRepository>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        
        _service = new AlunoService(_mockRepository.Object, _mockTurmaRepository.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAlunos()
    {
        // Arrange
        var alunos = new List<Aluno>
        {
            new Aluno { Id = 1, Nome = "Aluno 1", Matricula = "2025001" },
            new Aluno { Id = 2, Nome = "Aluno 2", Matricula = "2025002" }
        };

        _mockRepository.Setup(r => r.GetAllWithTurmaAsync()).ReturnsAsync(alunos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsAluno()
    {
        // Arrange
        var id = 1L;
        var aluno = new Aluno
        {
            Id = id,
            Nome = "Test Aluno",
            Matricula = "2025001"
        };

        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(id)).ReturnsAsync(aluno);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(aluno.Nome, result.Nome);
    }

    [Fact]
    public async Task GetByTurmaIdAsync_WithValidTurmaId_ReturnsAlunos()
    {
        // Arrange
        var turmaId = 1L;
        var turma = new Turma { Id = turmaId, Nome = "Turma A" };
        var alunos = new List<Aluno>
        {
            new Aluno { Id = 1, Nome = "Aluno 1", Matricula = "2025001", TurmaId = turmaId }
        };

        _mockTurmaRepository.Setup(r => r.GetByIdAsync(turmaId)).ReturnsAsync(turma);
        _mockRepository.Setup(r => r.GetByTurmaIdAsync(turmaId)).ReturnsAsync(alunos);

        // Act
        var result = await _service.GetByTurmaIdAsync(turmaId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByTurmaIdAsync_WithInvalidTurmaId_ThrowsNotFoundException()
    {
        // Arrange
        var turmaId = 999L;
        _mockTurmaRepository.Setup(r => r.GetByIdAsync(turmaId)).ReturnsAsync((Turma?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByTurmaIdAsync(turmaId));
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesAluno()
    {
        // Arrange
        var dto = new AlunoCreateDTO
        {
            Nome = "New Aluno",
            Matricula = "2025001",
            TurmaId = 1
        };

        var turma = new Turma { Id = 1, Nome = "Turma A" };

        _mockTurmaRepository.Setup(r => r.GetByIdAsync(dto.TurmaId)).ReturnsAsync(turma);
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Aluno, bool>>>()))
            .ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Aluno>()))
            .ReturnsAsync((Aluno a) => { a.Id = 1; return a; });

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Nome, result.Nome);
        Assert.Equal(dto.Matricula, result.Matricula);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Aluno>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidTurmaId_ThrowsNotFoundException()
    {
        // Arrange
        var dto = new AlunoCreateDTO
        {
            Nome = "New Aluno",
            Matricula = "2025001",
            TurmaId = 999
        };

        _mockTurmaRepository.Setup(r => r.GetByIdAsync(dto.TurmaId)).ReturnsAsync((Turma?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_WithExistingMatricula_ThrowsBusinessException()
    {
        // Arrange
        var dto = new AlunoCreateDTO
        {
            Nome = "New Aluno",
            Matricula = "2025001",
            TurmaId = 1
        };

        var turma = new Turma { Id = 1, Nome = "Turma A" };

        _mockTurmaRepository.Setup(r => r.GetByIdAsync(dto.TurmaId)).ReturnsAsync(turma);
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Aluno, bool>>>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _service.CreateAsync(dto));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Aluno>()), Times.Never);
    }
}
