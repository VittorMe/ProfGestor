using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProfGestor.Data;
using ProfGestor.DTOs;
using ProfGestor.Exceptions;
using ProfGestor.Repositories;

namespace ProfGestor.Services;

public class NotaAvaliacaoService : INotaAvaliacaoService
{
    private readonly INotaAvaliacaoRepository _repository;
    private readonly IAvaliacaoRepository _avaliacaoRepository;
    private readonly ITurmaRepository _turmaRepository;
    private readonly ProfGestorContext _context;
    private readonly IMapper _mapper;

    public NotaAvaliacaoService(
        INotaAvaliacaoRepository repository,
        IAvaliacaoRepository avaliacaoRepository,
        ITurmaRepository turmaRepository,
        ProfGestorContext context,
        IMapper mapper)
    {
        _repository = repository;
        _avaliacaoRepository = avaliacaoRepository;
        _turmaRepository = turmaRepository;
        _context = context;
        _mapper = mapper;
    }

    public async Task<LancamentoNotasResumoDTO> GetLancamentoNotasAsync(long avaliacaoId, long professorId)
    {
        // Buscar avaliação
        var avaliacao = await _avaliacaoRepository.GetByIdWithDetailsAsync(avaliacaoId);
        if (avaliacao == null)
            throw new NotFoundException("Avaliacao", avaliacaoId);

        // Verificar se a disciplina da avaliação pertence ao professor
        var turmas = await _turmaRepository.GetByProfessorIdAsync(professorId);
        var disciplinaIds = turmas.Select(t => t.DisciplinaId).Distinct().ToList();

        if (!disciplinaIds.Contains(avaliacao.DisciplinaId))
            throw new UnauthorizedAccessException("Avaliação não pertence ao professor");

        // Buscar turmas que têm essa disciplina e pertencem ao professor
        var turmasComDisciplina = turmas.Where(t => t.DisciplinaId == avaliacao.DisciplinaId).ToList();
        
        if (!turmasComDisciplina.Any())
            throw new NotFoundException("Nenhuma turma encontrada para esta disciplina");

        // Por enquanto, vamos usar a primeira turma. Em um cenário mais complexo, 
        // poderia haver múltiplas turmas com a mesma disciplina
        var turma = turmasComDisciplina.First();
        
        // Buscar todos os alunos da turma
        var alunos = await _context.Alunos
            .Where(a => a.TurmaId == turma.Id)
            .OrderBy(a => a.Nome)
            .ToListAsync();

        // Buscar notas já lançadas para esta avaliação
        var notasExistentes = await _repository.GetByAvaliacaoIdAsync(avaliacaoId);
        var notasDict = notasExistentes.ToDictionary(n => n.AlunoId, n => n);

        // Montar lista de alunos com notas
        var alunosNotas = alunos.Select(aluno =>
        {
            var temNota = notasDict.TryGetValue(aluno.Id, out var nota);
            var iniciais = ObterIniciais(aluno.Nome);
            
            return new AlunoNotaDTO
            {
                AlunoId = aluno.Id,
                AlunoNome = aluno.Nome,
                AlunoMatricula = aluno.Matricula,
                Iniciais = iniciais,
                Nota = temNota ? nota!.Valor : null,
                TemNota = temNota,
                DataLancamento = temNota ? nota!.DataLancamento : null
            };
        }).ToList();

        // Calcular média da turma
        var notasValidas = alunosNotas.Where(a => a.TemNota && a.Nota.HasValue).Select(a => a.Nota!.Value).ToList();
        var mediaTurma = notasValidas.Any() ? notasValidas.Average() : 0.0;

        return new LancamentoNotasResumoDTO
        {
            AvaliacaoId = avaliacao.Id,
            AvaliacaoTitulo = avaliacao.Titulo,
            DisciplinaNome = avaliacao.Disciplina.Nome,
            TurmaNome = turma.Nome,
            ValorMaximo = avaliacao.ValorMaximo,
            MediaTurma = mediaTurma,
            NotasLancadas = notasValidas.Count,
            TotalAlunos = alunosNotas.Count,
            Alunos = alunosNotas
        };
    }

    public async Task LancarNotasAsync(LancarNotasDTO dto, long professorId)
    {
        // Validar avaliação
        var avaliacao = await _avaliacaoRepository.GetByIdAsync(dto.AvaliacaoId);
        if (avaliacao == null)
            throw new NotFoundException("Avaliacao", dto.AvaliacaoId);

        // Verificar se a disciplina da avaliação pertence ao professor
        var turmas = await _turmaRepository.GetByProfessorIdAsync(professorId);
        var disciplinaIds = turmas.Select(t => t.DisciplinaId).Distinct().ToList();

        if (!disciplinaIds.Contains(avaliacao.DisciplinaId))
            throw new UnauthorizedAccessException("Avaliação não pertence ao professor");

        // Validar que os alunos pertencem às turmas do professor
        var turmaIds = turmas.Select(t => t.Id).ToList();
        var alunosIds = dto.Notas.Select(n => n.AlunoId).ToList();
        
        var alunosValidos = await _context.Alunos
            .Where(a => alunosIds.Contains(a.Id) && turmaIds.Contains(a.TurmaId))
            .Select(a => a.Id)
            .ToListAsync();

        if (alunosValidos.Count != alunosIds.Count)
            throw new BadRequestException("Alguns alunos não pertencem às turmas do professor");

        // Buscar notas existentes
        var notasExistentes = await _repository.GetByAvaliacaoIdAsync(dto.AvaliacaoId);
        var notasDict = notasExistentes.ToDictionary(n => n.AlunoId, n => n);

        // Processar cada nota
        foreach (var notaDto in dto.Notas)
        {
            // Validar valor
            if (notaDto.Valor < 0 || notaDto.Valor > avaliacao.ValorMaximo)
                throw new BadRequestException($"Nota inválida para aluno {notaDto.AlunoId}. Deve estar entre 0 e {avaliacao.ValorMaximo}");

            if (notasDict.TryGetValue(notaDto.AlunoId, out var notaExistente))
            {
                // Atualizar nota existente
                notaExistente.Valor = notaDto.Valor;
                notaExistente.DataLancamento = DateTime.Now;
                await _repository.UpdateAsync(notaExistente);
            }
            else
            {
                // Criar nova nota
                var novaNota = new Models.NotaAvaliacao
                {
                    Valor = notaDto.Valor,
                    AlunoId = notaDto.AlunoId,
                    AvaliacaoId = dto.AvaliacaoId,
                    DataLancamento = DateTime.Now,
                    Origem = "Manual"
                };
                await _repository.AddAsync(novaNota);
            }
        }
    }

    public async Task<NotaAvaliacaoDTO?> GetByIdAsync(long id)
    {
        var nota = await _repository.GetByIdAsync(id);
        if (nota == null)
            return null;

        return _mapper.Map<NotaAvaliacaoDTO>(nota);
    }

    public async Task<NotaAvaliacaoDTO> CreateAsync(NotaAvaliacaoCreateDTO dto)
    {
        // Validar aluno
        var aluno = await _context.Alunos.FindAsync(dto.AlunoId);
        if (aluno == null)
            throw new NotFoundException("Aluno", dto.AlunoId);

        // Validar avaliação
        var avaliacao = await _avaliacaoRepository.GetByIdAsync(dto.AvaliacaoId);
        if (avaliacao == null)
            throw new NotFoundException("Avaliacao", dto.AvaliacaoId);

        // Validar valor
        if (dto.Valor < 0 || dto.Valor > avaliacao.ValorMaximo)
            throw new BadRequestException($"Nota inválida. Deve estar entre 0 e {avaliacao.ValorMaximo}");

        var nota = _mapper.Map<Models.NotaAvaliacao>(dto);
        nota.DataLancamento = DateTime.Now;
        
        var created = await _repository.AddAsync(nota);
        var createdCompleta = await _context.NotasAvaliacao
            .Include(n => n.Aluno)
            .FirstOrDefaultAsync(n => n.Id == created.Id);

        return _mapper.Map<NotaAvaliacaoDTO>(createdCompleta!);
    }

    public async Task<NotaAvaliacaoDTO> UpdateAsync(long id, NotaAvaliacaoUpdateDTO dto)
    {
        var nota = await _repository.GetByIdAsync(id);
        if (nota == null)
            throw new NotFoundException("NotaAvaliacao", id);

        // Validar valor
        var avaliacao = await _avaliacaoRepository.GetByIdAsync(nota.AvaliacaoId);
        if (avaliacao != null && (dto.Valor < 0 || dto.Valor > avaliacao.ValorMaximo))
            throw new BadRequestException($"Nota inválida. Deve estar entre 0 e {avaliacao.ValorMaximo}");

        _mapper.Map(dto, nota);
        nota.DataLancamento = DateTime.Now;
        
        await _repository.UpdateAsync(nota);
        
        var updated = await _context.NotasAvaliacao
            .Include(n => n.Aluno)
            .FirstOrDefaultAsync(n => n.Id == id);

        return _mapper.Map<NotaAvaliacaoDTO>(updated!);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var nota = await _repository.GetByIdAsync(id);
        if (nota == null)
            throw new NotFoundException("NotaAvaliacao", id);

        await _repository.DeleteAsync(nota);
        return true;
    }

    private string ObterIniciais(string nome)
    {
        var partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length == 0)
            return "??";
        
        if (partes.Length == 1)
            return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();
        
        return (partes[0][0].ToString() + partes[partes.Length - 1][0].ToString()).ToUpper();
    }
}

