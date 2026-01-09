using Moq;
using AutoMapper;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Models;
using ProfGestor.Repositories;
using ProfGestor.Services;
using ProfGestor.Mappings;

namespace TestProfGestor.Services;

public class TurmaServiceTests
{
    private readonly Mock<ITurmaRepository> _mockRepository;
    private readonly Mock<IProfessorRepository> _mockProfessorRepository;
    private readonly Mock<IRepository<Disciplina>> _mockDisciplinaRepository;
    private readonly IMapper _mapper;
    private readonly TurmaService _service;

    public TurmaServiceTests()
    {
        _mockRepository = new Mock<ITurmaRepository>();
        _mockProfessorRepository = new Mock<IProfessorRepository>();
        _mockDisciplinaRepository = new Mock<IRepository<Disciplina>>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        
        _service = new TurmaService(
            _mockRepository.Object,
            _mockProfessorRepository.Object,
            _mockDisciplinaRepository.Object,
            _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTurmas()
    {
        // Arrange
        var turmas = new List<Turma>
        {
            new Turma { Id = 1, Nome = "Turma A", AnoLetivo = 2025, Semestre = 1 },
            new Turma { Id = 2, Nome = "Turma B", AnoLetivo = 2025, Semestre = 1 }
        };

        _mockRepository.Setup(r => r.GetAllWithDetailsAsync()).ReturnsAsync(turmas);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsTurma()
    {
        // Arrange
        var id = 1L;
        var turma = new Turma
        {
            Id = id,
            Nome = "Turma A",
            AnoLetivo = 2025,
            Semestre = 1
        };

        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(id)).ReturnsAsync(turma);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(turma.Nome, result.Nome);
    }

    [Fact]
    public async Task GetByProfessorIdAsync_WithValidProfessorId_ReturnsTurmas()
    {
        // Arrange
        var professorId = 1L;
        var professor = new Professor { Id = professorId, Nome = "Test Professor" };
        var turmas = new List<Turma>
        {
            new Turma { Id = 1, Nome = "Turma A", ProfessorId = professorId }
        };

        _mockProfessorRepository.Setup(r => r.GetByIdAsync(professorId)).ReturnsAsync(professor);
        _mockRepository.Setup(r => r.GetByProfessorIdAsync(professorId)).ReturnsAsync(turmas);

        // Act
        var result = await _service.GetByProfessorIdAsync(professorId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByProfessorIdAsync_WithInvalidProfessorId_ThrowsNotFoundException()
    {
        // Arrange
        var professorId = 999L;
        _mockProfessorRepository.Setup(r => r.GetByIdAsync(professorId)).ReturnsAsync((Professor?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByProfessorIdAsync(professorId));
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesTurma()
    {
        // Arrange
        var dto = new TurmaCreateDTO
        {
            Nome = "Turma A",
            AnoLetivo = 2025,
            Semestre = 1,
            Turno = "Manhã",
            QtdAlunos = 30,
            ProfessorId = 1,
            DisciplinaId = 1
        };

        var professor = new Professor { Id = 1, Nome = "Test Professor" };
        var disciplina = new Disciplina { Id = 1, Nome = "Matemática" };

        _mockProfessorRepository.Setup(r => r.GetByIdAsync(dto.ProfessorId)).ReturnsAsync(professor);
        _mockDisciplinaRepository.Setup(r => r.GetByIdAsync(dto.DisciplinaId)).ReturnsAsync(disciplina);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Turma>()))
            .ReturnsAsync((Turma t) => 
            { 
                t.Id = 1; 
                return t; 
            });

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Nome, result.Nome);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Turma>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidProfessorId_ThrowsNotFoundException()
    {
        // Arrange
        var dto = new TurmaCreateDTO
        {
            Nome = "Turma A",
            AnoLetivo = 2025,
            Semestre = 1,
            Turno = "Manhã",
            QtdAlunos = 30,
            ProfessorId = 999,
            DisciplinaId = 1
        };

        _mockProfessorRepository.Setup(r => r.GetByIdAsync(dto.ProfessorId)).ReturnsAsync((Professor?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(dto));
    }
}
