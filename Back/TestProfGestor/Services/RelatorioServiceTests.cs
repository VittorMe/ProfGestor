using Microsoft.EntityFrameworkCore;
using Moq;
using ProfGestor.Data;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Models;
using ProfGestor.Models.Enums;
using ProfGestor.Repositories;
using ProfGestor.Services;

namespace TestProfGestor.Services;

public class RelatorioServiceTests : IDisposable
{
    private readonly ProfGestorContext _context;
    private readonly RelatorioService _service;
    private readonly Mock<ITurmaRepository> _mockTurmaRepository;

    public RelatorioServiceTests()
    {
        // Configurar banco em memória para testes
        var options = new DbContextOptionsBuilder<ProfGestorContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ProfGestorContext(options);
        _mockTurmaRepository = new Mock<ITurmaRepository>();

        var mockAulaRepository = new Mock<IAulaRepository>();
        var mockFrequenciaRepository = new Mock<IFrequenciaRepository>();
        var mockAvaliacaoRepository = new Mock<IAvaliacaoRepository>();
        var mockNotaAvaliacaoRepository = new Mock<INotaAvaliacaoRepository>();

        _service = new RelatorioService(
            _context,
            _mockTurmaRepository.Object,
            mockAulaRepository.Object,
            mockFrequenciaRepository.Object,
            mockAvaliacaoRepository.Object,
            mockNotaAvaliacaoRepository.Object);
    }

    [Fact]
    public async Task GerarRelatorioFrequenciaAsync_WithValidData_ReturnsRelatorio()
    {
        // Arrange
        var disciplina = new Disciplina { Id = 1, Nome = "Matemática" };
        var turma = new Turma
        {
            Id = 1,
            Nome = "3º Ano A",
            AnoLetivo = 2025,
            Semestre = 1,
            Turno = "Manhã",
            QtdAlunos = 2,
            DisciplinaId = 1,
            Disciplina = disciplina
        };

        var alunos = new List<Aluno>
        {
            new Aluno { Id = 1, Nome = "Aluno 1", Matricula = "2025001", TurmaId = 1 },
            new Aluno { Id = 2, Nome = "Aluno 2", Matricula = "2025002", TurmaId = 1 }
        };

        var aulas = new List<Aula>
        {
            new Aula { Id = 1, Data = new DateOnly(2025, 1, 10), Periodo = "Manhã", TurmaId = 1 },
            new Aula { Id = 2, Data = new DateOnly(2025, 1, 13), Periodo = "Manhã", TurmaId = 1 }
        };

        var frequencias = new List<Frequencia>
        {
            new Frequencia { Id = 1, Status = StatusFrequencia.PRESENTE, AlunoId = 1, AulaId = 1, Aluno = alunos[0] },
            new Frequencia { Id = 2, Status = StatusFrequencia.PRESENTE, AlunoId = 1, AulaId = 2, Aluno = alunos[0] },
            new Frequencia { Id = 3, Status = StatusFrequencia.FALTA, AlunoId = 2, AulaId = 1, Aluno = alunos[1] },
            new Frequencia { Id = 4, Status = StatusFrequencia.PRESENTE, AlunoId = 2, AulaId = 2, Aluno = alunos[1] }
        };

        _context.Disciplinas.Add(disciplina);
        _context.Turmas.Add(turma);
        _context.Alunos.AddRange(alunos);
        _context.Aulas.AddRange(aulas);
        _context.Frequencias.AddRange(frequencias);
        await _context.SaveChangesAsync();

        _mockTurmaRepository.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(turma);

        var request = new RelatorioFrequenciaRequestDTO
        {
            TurmaId = 1,
            DataInicio = new DateOnly(2025, 1, 10),
            DataFim = new DateOnly(2025, 1, 13)
        };

        // Act
        var result = await _service.GerarRelatorioFrequenciaAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(turma.Id, result.TurmaId);
        Assert.Equal(turma.Nome, result.TurmaNome);
        Assert.Equal(disciplina.Nome, result.DisciplinaNome);
        Assert.Equal(2, result.TotalAulas);
        Assert.Equal(2, result.Alunos.Count);
        Assert.Equal(3, result.TotalPresencas);
        Assert.Equal(1, result.TotalFaltas);
        Assert.Equal(100.0, result.Alunos.First(a => a.AlunoId == 1).PercentualPresenca);
        Assert.Equal(50.0, result.Alunos.First(a => a.AlunoId == 2).PercentualPresenca);
    }

    [Fact]
    public async Task GerarRelatorioFrequenciaAsync_WithInvalidTurmaId_ThrowsNotFoundException()
    {
        // Arrange
        _mockTurmaRepository.Setup(r => r.GetByIdWithDetailsAsync(999))
            .ReturnsAsync((Turma?)null);

        var request = new RelatorioFrequenciaRequestDTO
        {
            TurmaId = 999,
            DataInicio = new DateOnly(2025, 1, 10),
            DataFim = new DateOnly(2025, 1, 13)
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _service.GerarRelatorioFrequenciaAsync(request));
    }

    [Fact]
    public async Task GerarRelatorioFrequenciaAsync_WithInvalidDateRange_ThrowsBadRequestException()
    {
        // Arrange
        var turma = new Turma
        {
            Id = 1,
            Nome = "3º Ano A",
            AnoLetivo = 2025,
            Semestre = 1,
            Turno = "Manhã",
            QtdAlunos = 0,
            DisciplinaId = 1
        };

        _mockTurmaRepository.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(turma);

        var request = new RelatorioFrequenciaRequestDTO
        {
            TurmaId = 1,
            DataInicio = new DateOnly(2025, 1, 13), // Data início maior que fim
            DataFim = new DateOnly(2025, 1, 10)
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => 
            _service.GerarRelatorioFrequenciaAsync(request));
    }

    [Fact]
    public async Task GerarRelatorioFrequenciaAsync_WithNoAulas_ReturnsRelatorioWithZeroAulas()
    {
        // Arrange
        var disciplina = new Disciplina { Id = 1, Nome = "Matemática" };
        var turma = new Turma
        {
            Id = 1,
            Nome = "3º Ano A",
            AnoLetivo = 2025,
            Semestre = 1,
            Turno = "Manhã",
            QtdAlunos = 1,
            DisciplinaId = 1,
            Disciplina = disciplina
        };

        var aluno = new Aluno { Id = 1, Nome = "Aluno 1", Matricula = "2025001", TurmaId = 1 };

        _context.Disciplinas.Add(disciplina);
        _context.Turmas.Add(turma);
        _context.Alunos.Add(aluno);
        await _context.SaveChangesAsync();

        _mockTurmaRepository.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(turma);

        var request = new RelatorioFrequenciaRequestDTO
        {
            TurmaId = 1,
            DataInicio = new DateOnly(2025, 1, 10),
            DataFim = new DateOnly(2025, 1, 13)
        };

        // Act
        var result = await _service.GerarRelatorioFrequenciaAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalAulas);
        Assert.Equal(0, result.MediaPresenca);
        Assert.Single(result.Alunos);
        Assert.Equal(0, result.Alunos[0].PercentualPresenca);
    }

    [Fact]
    public async Task GerarRelatorioDesempenhoAsync_WithValidData_ReturnsRelatorio()
    {
        // Arrange
        var disciplina = new Disciplina { Id = 1, Nome = "Matemática" };
        var turma = new Turma
        {
            Id = 1,
            Nome = "3º Ano A",
            AnoLetivo = 2025,
            Semestre = 1,
            Turno = "Manhã",
            QtdAlunos = 2,
            DisciplinaId = 1,
            Disciplina = disciplina
        };

        var alunos = new List<Aluno>
        {
            new Aluno { Id = 1, Nome = "Aluno 1", Matricula = "2025001", TurmaId = 1 },
            new Aluno { Id = 2, Nome = "Aluno 2", Matricula = "2025002", TurmaId = 1 }
        };

        var avaliacoes = new List<Avaliacao>
        {
            new Avaliacao
            {
                Id = 1,
                Titulo = "Avaliação 1",
                Tipo = TipoAvaliacao.PROVA,
                DataAplicacao = new DateOnly(2025, 1, 15),
                ValorMaximo = 10.0,
                DisciplinaId = 1
            },
            new Avaliacao
            {
                Id = 2,
                Titulo = "Avaliação 2",
                Tipo = TipoAvaliacao.PROVA,
                DataAplicacao = new DateOnly(2025, 1, 20),
                ValorMaximo = 10.0,
                DisciplinaId = 1
            }
        };

        var notas = new List<NotaAvaliacao>
        {
            new NotaAvaliacao { Id = 1, Valor = 8.0, AlunoId = 1, AvaliacaoId = 1, DataLancamento = DateTime.Now, Origem = "Manual" },
            new NotaAvaliacao { Id = 2, Valor = 9.0, AlunoId = 1, AvaliacaoId = 2, DataLancamento = DateTime.Now, Origem = "Manual" },
            new NotaAvaliacao { Id = 3, Valor = 6.0, AlunoId = 2, AvaliacaoId = 1, DataLancamento = DateTime.Now, Origem = "Manual" },
            new NotaAvaliacao { Id = 4, Valor = 7.0, AlunoId = 2, AvaliacaoId = 2, DataLancamento = DateTime.Now, Origem = "Manual" }
        };

        _context.Disciplinas.Add(disciplina);
        _context.Turmas.Add(turma);
        _context.Alunos.AddRange(alunos);
        _context.Avaliacoes.AddRange(avaliacoes);
        _context.NotasAvaliacao.AddRange(notas);
        await _context.SaveChangesAsync();

        _mockTurmaRepository.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(turma);

        var request = new RelatorioDesempenhoRequestDTO
        {
            TurmaId = 1,
            Periodo = null
        };

        // Act
        var result = await _service.GerarRelatorioDesempenhoAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(turma.Id, result.TurmaId);
        Assert.Equal(turma.Nome, result.TurmaNome);
        Assert.Equal(disciplina.Nome, result.DisciplinaNome);
        Assert.Equal(2, result.Alunos.Count);
        Assert.True(result.MediaGeralTurma > 0);
        Assert.True(result.MaiorNota > 0);
        Assert.True(result.MenorNota > 0);
        Assert.NotNull(result.DistribuicaoNotas);
        Assert.NotNull(result.ClassificacaoDesempenho);
        Assert.NotNull(result.Observacao);
        Assert.NotNull(result.Recomendacao);
    }

    [Fact]
    public async Task GerarRelatorioDesempenhoAsync_WithInvalidTurmaId_ThrowsNotFoundException()
    {
        // Arrange
        _mockTurmaRepository.Setup(r => r.GetByIdWithDetailsAsync(999))
            .ReturnsAsync((Turma?)null);

        var request = new RelatorioDesempenhoRequestDTO
        {
            TurmaId = 999,
            Periodo = null
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _service.GerarRelatorioDesempenhoAsync(request));
    }

    [Fact]
    public async Task GerarRelatorioDesempenhoAsync_WithNoAvaliacoes_ReturnsRelatorioWithZeroMedia()
    {
        // Arrange
        var disciplina = new Disciplina { Id = 1, Nome = "Matemática" };
        var turma = new Turma
        {
            Id = 1,
            Nome = "3º Ano A",
            AnoLetivo = 2025,
            Semestre = 1,
            Turno = "Manhã",
            QtdAlunos = 1,
            DisciplinaId = 1,
            Disciplina = disciplina
        };

        var aluno = new Aluno { Id = 1, Nome = "Aluno 1", Matricula = "2025001", TurmaId = 1 };

        _context.Disciplinas.Add(disciplina);
        _context.Turmas.Add(turma);
        _context.Alunos.Add(aluno);
        await _context.SaveChangesAsync();

        _mockTurmaRepository.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(turma);

        var request = new RelatorioDesempenhoRequestDTO
        {
            TurmaId = 1,
            Periodo = null
        };

        // Act
        var result = await _service.GerarRelatorioDesempenhoAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.MediaGeralTurma);
        Assert.Equal(0, result.MedianaTurma);
        Assert.Single(result.Alunos);
        Assert.Equal(0, result.Alunos[0].MediaGeral);
    }

    [Fact]
    public async Task GerarRelatorioDesempenhoAsync_WithPeriodo_FiltersAvaliacoes()
    {
        // Arrange
        var disciplina = new Disciplina { Id = 1, Nome = "Matemática" };
        var turma = new Turma
        {
            Id = 1,
            Nome = "3º Ano A",
            AnoLetivo = 2025,
            Semestre = 1,
            Turno = "Manhã",
            QtdAlunos = 1,
            DisciplinaId = 1,
            Disciplina = disciplina
        };

        var aluno = new Aluno { Id = 1, Nome = "Aluno 1", Matricula = "2025001", TurmaId = 1 };

        var aulas = new List<Aula>
        {
            new Aula { Id = 1, Data = new DateOnly(2025, 1, 10), Periodo = "1º Bimestre", TurmaId = 1 },
            new Aula { Id = 2, Data = new DateOnly(2025, 1, 15), Periodo = "1º Bimestre", TurmaId = 1 }
        };

        var avaliacoes = new List<Avaliacao>
        {
            new Avaliacao
            {
                Id = 1,
                Titulo = "Avaliação 1º Bimestre",
                Tipo = TipoAvaliacao.PROVA,
                DataAplicacao = new DateOnly(2025, 1, 12),
                ValorMaximo = 10.0,
                DisciplinaId = 1
            },
            new Avaliacao
            {
                Id = 2,
                Titulo = "Avaliação 2º Bimestre",
                Tipo = TipoAvaliacao.PROVA,
                DataAplicacao = new DateOnly(2025, 3, 15),
                ValorMaximo = 10.0,
                DisciplinaId = 1
            }
        };

        _context.Disciplinas.Add(disciplina);
        _context.Turmas.Add(turma);
        _context.Alunos.Add(aluno);
        _context.Aulas.AddRange(aulas);
        _context.Avaliacoes.AddRange(avaliacoes);
        await _context.SaveChangesAsync();

        _mockTurmaRepository.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(turma);

        var request = new RelatorioDesempenhoRequestDTO
        {
            TurmaId = 1,
            Periodo = "1º Bimestre"
        };

        // Act
        var result = await _service.GerarRelatorioDesempenhoAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("1º Bimestre", result.Periodo);
        // Deve filtrar apenas avaliações do período
        Assert.Single(result.Alunos[0].Avaliacoes);
        Assert.Equal("Avaliação 1º Bimestre", result.Alunos[0].Avaliacoes[0].Titulo);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
