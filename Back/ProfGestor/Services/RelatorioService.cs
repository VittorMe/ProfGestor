using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Models.Enums;
using ProfGestor.Repositories;

namespace ProfGestor.Services;

public class RelatorioService : IRelatorioService
{
    private readonly ProfGestorContext _context;
    private readonly ITurmaRepository _turmaRepository;
    private readonly IAulaRepository _aulaRepository;
    private readonly IFrequenciaRepository _frequenciaRepository;
    private readonly IAvaliacaoRepository _avaliacaoRepository;
    private readonly INotaAvaliacaoRepository _notaAvaliacaoRepository;

    public RelatorioService(
        ProfGestorContext context,
        ITurmaRepository turmaRepository,
        IAulaRepository aulaRepository,
        IFrequenciaRepository frequenciaRepository,
        IAvaliacaoRepository avaliacaoRepository,
        INotaAvaliacaoRepository notaAvaliacaoRepository)
    {
        _context = context;
        _turmaRepository = turmaRepository;
        _aulaRepository = aulaRepository;
        _frequenciaRepository = frequenciaRepository;
        _avaliacaoRepository = avaliacaoRepository;
        _notaAvaliacaoRepository = notaAvaliacaoRepository;
    }

    public async Task<RelatorioFrequenciaDTO> GerarRelatorioFrequenciaAsync(RelatorioFrequenciaRequestDTO request)
    {
        // Validar turma com disciplina
        var turma = await _turmaRepository.GetByIdWithDetailsAsync(request.TurmaId);
        if (turma == null)
            throw new NotFoundException("Turma", request.TurmaId);

        // Validar período
        if (request.DataInicio > request.DataFim)
            throw new BadRequestException("A data de início deve ser anterior à data de fim.");

        // Buscar aulas no período
        var aulas = await _context.Aulas
            .Where(a => a.TurmaId == request.TurmaId &&
                       a.Data >= request.DataInicio &&
                       a.Data <= request.DataFim)
            .OrderBy(a => a.Data)
            .ToListAsync();

        var totalAulas = aulas.Count;

        // Buscar alunos da turma
        var alunos = await _context.Alunos
            .Where(a => a.TurmaId == request.TurmaId)
            .OrderBy(a => a.Nome)
            .ToListAsync();

        // Buscar todas as frequências das aulas no período
        var aulaIds = aulas.Select(a => a.Id).ToList();
        var frequencias = await _context.Frequencias
            .Include(f => f.Aluno)
            .Where(f => aulaIds.Contains(f.AulaId))
            .ToListAsync();

        // Calcular estatísticas por aluno
        var alunosRelatorio = alunos.Select(aluno =>
        {
            var frequenciasAluno = frequencias
                .Where(f => f.AlunoId == aluno.Id)
                .ToList();

            var presencas = frequenciasAluno.Count(f => f.Status == StatusFrequencia.PRESENTE);
            var faltas = frequenciasAluno.Count(f => f.Status == StatusFrequencia.FALTA);
            var faltasJustificadas = frequenciasAluno.Count(f => f.Status == StatusFrequencia.FALTA_JUSTIFICADA);

            var percentualPresenca = totalAulas > 0
                ? (double)presencas / totalAulas * 100
                : 0;

            return new AlunoFrequenciaRelatorioDTO
            {
                AlunoId = aluno.Id,
                AlunoNome = aluno.Nome,
                Matricula = aluno.Matricula,
                TotalAulas = totalAulas,
                Presencas = presencas,
                Faltas = faltas,
                FaltasJustificadas = faltasJustificadas,
                PercentualPresenca = Math.Round(percentualPresenca, 2)
            };
        }).ToList();

        // Calcular média geral de presença
        var mediaPresenca = alunosRelatorio.Any()
            ? alunosRelatorio.Average(a => a.PercentualPresenca)
            : 0;

        // Calcular totais
        var totalPresencas = alunosRelatorio.Sum(a => a.Presencas);
        var totalFaltas = alunosRelatorio.Sum(a => a.Faltas);
        var totalFaltasJustificadas = alunosRelatorio.Sum(a => a.FaltasJustificadas);

        return new RelatorioFrequenciaDTO
        {
            TurmaId = turma.Id,
            TurmaNome = turma.Nome,
            DisciplinaNome = turma.Disciplina?.Nome ?? string.Empty,
            DataInicio = request.DataInicio,
            DataFim = request.DataFim,
            GeradoEm = DateTime.Now,
            Alunos = alunosRelatorio,
            TotalAulas = totalAulas,
            MediaPresenca = Math.Round(mediaPresenca, 2),
            TotalPresencas = totalPresencas,
            TotalFaltas = totalFaltas,
            TotalFaltasJustificadas = totalFaltasJustificadas
        };
    }

    public async Task<RelatorioDesempenhoDTO> GerarRelatorioDesempenhoAsync(RelatorioDesempenhoRequestDTO request)
    {
        // Validar turma com disciplina
        var turma = await _turmaRepository.GetByIdWithDetailsAsync(request.TurmaId);
        if (turma == null)
            throw new NotFoundException("Turma", request.TurmaId);

        // Buscar avaliações da disciplina da turma
        var avaliacoesQuery = _context.Avaliacoes
            .Where(a => a.DisciplinaId == turma.DisciplinaId);

        // Se período foi informado, filtrar por período (usando o campo Periodo da Aula como referência)
        if (!string.IsNullOrWhiteSpace(request.Periodo))
        {
            // Buscar aulas do período para obter as datas
            var aulasPeriodo = await _context.Aulas
                .Where(a => a.TurmaId == request.TurmaId && a.Periodo == request.Periodo)
                .Select(a => a.Data)
                .ToListAsync();

            if (aulasPeriodo.Any())
            {
                var dataInicio = aulasPeriodo.Min();
                var dataFim = aulasPeriodo.Max();
                avaliacoesQuery = avaliacoesQuery
                    .Where(a => a.DataAplicacao >= dataInicio && a.DataAplicacao <= dataFim);
            }
        }

        var avaliacoes = await avaliacoesQuery
            .OrderBy(a => a.DataAplicacao)
            .ToListAsync();

        // Buscar alunos da turma
        var alunos = await _context.Alunos
            .Where(a => a.TurmaId == request.TurmaId)
            .OrderBy(a => a.Nome)
            .ToListAsync();

        // Buscar todas as notas das avaliações
        var avaliacaoIds = avaliacoes.Select(a => a.Id).ToList();
        var notas = await _context.NotasAvaliacao
            .Where(n => avaliacaoIds.Contains(n.AvaliacaoId))
            .ToListAsync();

        // Calcular desempenho por aluno
        var alunosRelatorio = alunos.Select(aluno =>
        {
            var avaliacoesAluno = avaliacoes.Select(avaliacao =>
            {
                var nota = notas.FirstOrDefault(n => n.AvaliacaoId == avaliacao.Id && n.AlunoId == aluno.Id);
                double? percentual = nota != null && avaliacao.ValorMaximo > 0
                    ? (nota.Valor / avaliacao.ValorMaximo) * 100
                    : null;

                return new AvaliacaoDesempenhoDTO
                {
                    AvaliacaoId = avaliacao.Id,
                    Titulo = avaliacao.Titulo,
                    DataAplicacao = avaliacao.DataAplicacao,
                    ValorMaximo = avaliacao.ValorMaximo,
                    Nota = nota?.Valor,
                    Percentual = percentual.HasValue ? Math.Round(percentual.Value, 2) : null
                };
            }).ToList();

            var notasAluno = avaliacoesAluno
                .Where(a => a.Nota.HasValue)
                .Select(a => a.Nota!.Value)
                .ToList();

            var somaNotas = notasAluno.Sum();
            var somaValorMaximo = avaliacoesAluno
                .Where(a => a.Nota.HasValue)
                .Sum(a => a.ValorMaximo);

            var mediaGeral = somaValorMaximo > 0
                ? (somaNotas / somaValorMaximo) * 100
                : 0;

            return new AlunoDesempenhoRelatorioDTO
            {
                AlunoId = aluno.Id,
                AlunoNome = aluno.Nome,
                Matricula = aluno.Matricula,
                TotalAvaliacoes = avaliacoes.Count,
                MediaGeral = Math.Round(mediaGeral, 2),
                SomaNotas = Math.Round(somaNotas, 2),
                SomaValorMaximo = somaValorMaximo,
                Avaliacoes = avaliacoesAluno
            };
        }).ToList();

        // Calcular estatísticas da turma
        var mediasAlunos = alunosRelatorio
            .Where(a => a.MediaGeral > 0)
            .Select(a => a.MediaGeral)
            .OrderBy(m => m)
            .ToList();

        var mediaGeralTurma = mediasAlunos.Any()
            ? mediasAlunos.Average()
            : 0;

        var medianaTurma = mediasAlunos.Any()
            ? mediasAlunos.Count % 2 == 0
                ? (mediasAlunos[mediasAlunos.Count / 2 - 1] + mediasAlunos[mediasAlunos.Count / 2]) / 2
                : mediasAlunos[mediasAlunos.Count / 2]
            : 0;

        var qtdAcimaMedia = mediasAlunos.Count(m => m > mediaGeralTurma);
        var qtdAbaixoMedia = mediasAlunos.Count(m => m < mediaGeralTurma);

        // Calcular maior e menor nota
        var maiorNota = mediasAlunos.Any() ? mediasAlunos.Max() : 0;
        var menorNota = mediasAlunos.Any() ? mediasAlunos.Min() : 0;

        // Distribuição de notas por faixas
        var distribuicaoNotas = new List<DistribuicaoNotasDTO>
        {
            new DistribuicaoNotasDTO { Faixa = "0-3", Quantidade = mediasAlunos.Count(m => m >= 0 && m < 3) },
            new DistribuicaoNotasDTO { Faixa = "3-5", Quantidade = mediasAlunos.Count(m => m >= 3 && m < 5) },
            new DistribuicaoNotasDTO { Faixa = "5-7", Quantidade = mediasAlunos.Count(m => m >= 5 && m < 7) },
            new DistribuicaoNotasDTO { Faixa = "7-9", Quantidade = mediasAlunos.Count(m => m >= 7 && m < 9) },
            new DistribuicaoNotasDTO { Faixa = "9-10", Quantidade = mediasAlunos.Count(m => m >= 9 && m <= 10) }
        };

        // Classificação de desempenho
        var totalAlunos = mediasAlunos.Count;
        var classificacaoDesempenho = new List<ClassificacaoDesempenhoDTO>
        {
            new ClassificacaoDesempenhoDTO
            {
                Categoria = "Insuficiente",
                Faixa = "0-5",
                Quantidade = mediasAlunos.Count(m => m >= 0 && m < 5),
                Percentual = totalAlunos > 0 ? Math.Round((double)mediasAlunos.Count(m => m >= 0 && m < 5) / totalAlunos * 100, 1) : 0
            },
            new ClassificacaoDesempenhoDTO
            {
                Categoria = "Regular",
                Faixa = "5-7",
                Quantidade = mediasAlunos.Count(m => m >= 5 && m < 7),
                Percentual = totalAlunos > 0 ? Math.Round((double)mediasAlunos.Count(m => m >= 5 && m < 7) / totalAlunos * 100, 1) : 0
            },
            new ClassificacaoDesempenhoDTO
            {
                Categoria = "Bom",
                Faixa = "7-9",
                Quantidade = mediasAlunos.Count(m => m >= 7 && m < 9),
                Percentual = totalAlunos > 0 ? Math.Round((double)mediasAlunos.Count(m => m >= 7 && m < 9) / totalAlunos * 100, 1) : 0
            },
            new ClassificacaoDesempenhoDTO
            {
                Categoria = "Excelente",
                Faixa = "9-10",
                Quantidade = mediasAlunos.Count(m => m >= 9 && m <= 10),
                Percentual = totalAlunos > 0 ? Math.Round((double)mediasAlunos.Count(m => m >= 9 && m <= 10) / totalAlunos * 100, 1) : 0
            }
        };

        // Gerar observação e recomendação
        var observacao = $"A turma apresenta média geral de {mediaGeralTurma:F1}, indicando um desempenho {(mediaGeralTurma >= 7 ? "satisfatório" : mediaGeralTurma >= 5 ? "regular" : "insuficiente")}. " +
                        $"No entanto, {qtdAbaixoMedia} alunos estão abaixo da média e podem necessitar de atenção especial.";

        var recomendacao = qtdAbaixoMedia > 0
            ? $"Considere intervenções pedagógicas para os alunos com desempenho abaixo de 5.0, incluindo atividades de reforço e acompanhamento individualizado."
            : "A turma apresenta bom desempenho geral. Continue mantendo o nível de engajamento e considerando atividades de aprofundamento para os alunos com melhor desempenho.";

        return new RelatorioDesempenhoDTO
        {
            TurmaId = turma.Id,
            TurmaNome = turma.Nome,
            DisciplinaNome = turma.Disciplina?.Nome ?? string.Empty,
            Periodo = request.Periodo,
            GeradoEm = DateTime.Now,
            Alunos = alunosRelatorio,
            MediaGeralTurma = Math.Round(mediaGeralTurma, 2),
            MedianaTurma = Math.Round(medianaTurma, 2),
            MaiorNota = Math.Round(maiorNota, 2),
            MenorNota = Math.Round(menorNota, 2),
            QtdAcimaMedia = qtdAcimaMedia,
            QtdAbaixoMedia = qtdAbaixoMedia,
            DistribuicaoNotas = distribuicaoNotas,
            ClassificacaoDesempenho = classificacaoDesempenho,
            Observacao = observacao,
            Recomendacao = recomendacao
        };
    }
}
