using Microsoft.EntityFrameworkCore;
using ProfGestor.Models;
using ProfGestor.Models.Enums;

namespace ProfGestor.Data;

public class ProfGestorContext : DbContext
{
    public ProfGestorContext(DbContextOptions<ProfGestorContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Professor> Professores { get; set; }
    public DbSet<Disciplina> Disciplinas { get; set; }
    public DbSet<Turma> Turmas { get; set; }
    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Aula> Aulas { get; set; }
    public DbSet<PlanejamentoAula> PlanejamentosAula { get; set; }
    public DbSet<MaterialDidatico> MateriaisDidaticos { get; set; }
    public DbSet<AnotacaoPlanejamento> AnotacoesPlanejamento { get; set; }
    public DbSet<EtiquetaPlanejamento> EtiquetasPlanejamento { get; set; }
    public DbSet<Avaliacao> Avaliacoes { get; set; }
    public DbSet<QuestaoObjetiva> QuestoesObjetivas { get; set; }
    public DbSet<GabaritoQuestao> GabaritosQuestao { get; set; }
    public DbSet<NotaAvaliacao> NotasAvaliacao { get; set; }
    public DbSet<RespostaAluno> RespostasAluno { get; set; }
    public DbSet<Frequencia> Frequencias { get; set; }
    public DbSet<AnotacaoAula> AnotacoesAula { get; set; }
    public DbSet<RelatorioDesempenhoTurma> RelatoriosDesempenhoTurma { get; set; }
    public DbSet<LinhaRelatorioDesempenho> LinhasRelatorioDesempenho { get; set; }
    public DbSet<RelatorioFrequencia> RelatoriosFrequencia { get; set; }
    public DbSet<LinhaRelatorioFrequencia> LinhasRelatorioFrequencia { get; set; }
    public DbSet<Log> Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração de nomes de tabelas (singular conforme script SQL)
        modelBuilder.Entity<Professor>().ToTable("Professor");
        modelBuilder.Entity<Disciplina>().ToTable("Disciplina");
        modelBuilder.Entity<Turma>().ToTable("Turma");
        modelBuilder.Entity<Aluno>().ToTable("Aluno");
        modelBuilder.Entity<Aula>().ToTable("Aula");
        modelBuilder.Entity<PlanejamentoAula>().ToTable("PlanejamentoAula");
        modelBuilder.Entity<MaterialDidatico>().ToTable("MaterialDidatico");
        modelBuilder.Entity<AnotacaoPlanejamento>().ToTable("AnotacaoPlanejamento");
        modelBuilder.Entity<EtiquetaPlanejamento>().ToTable("EtiquetaPlanejamento");
        modelBuilder.Entity<Avaliacao>().ToTable("Avaliacao");
        modelBuilder.Entity<QuestaoObjetiva>().ToTable("QuestaoObjetiva");
        modelBuilder.Entity<GabaritoQuestao>().ToTable("GabaritoQuestao");
        modelBuilder.Entity<NotaAvaliacao>().ToTable("NotaAvaliacao");
        modelBuilder.Entity<RespostaAluno>().ToTable("RespostaAluno");
        modelBuilder.Entity<Frequencia>().ToTable("Frequencia");
        modelBuilder.Entity<AnotacaoAula>().ToTable("AnotacaoAula");
        modelBuilder.Entity<RelatorioDesempenhoTurma>().ToTable("RelatorioDesempenhoTurma");
        modelBuilder.Entity<LinhaRelatorioDesempenho>().ToTable("LinhaRelatorioDesempenho");
        modelBuilder.Entity<RelatorioFrequencia>().ToTable("RelatorioFrequencia");
        modelBuilder.Entity<LinhaRelatorioFrequencia>().ToTable("LinhaRelatorioFrequencia");
        modelBuilder.Entity<Log>().ToTable("Log");

        // Configuração de nomes de colunas (camelCase conforme script SQL)
        // Usando uma abordagem mais eficiente: mapear todas as propriedades de uma vez
        ConfigureColumnNames(modelBuilder);

        // Configuração de Enums
        modelBuilder.Entity<Avaliacao>()
            .Property(a => a.Tipo)
            .HasConversion<string>();

        modelBuilder.Entity<Frequencia>()
            .Property(f => f.Status)
            .HasConversion<string>();

        // Configuração de relacionamentos únicos
        modelBuilder.Entity<GabaritoQuestao>()
            .HasIndex(g => g.QuestaoObjetivaId)
            .IsUnique();

        modelBuilder.Entity<AnotacaoAula>()
            .HasIndex(a => a.AulaId)
            .IsUnique();

        modelBuilder.Entity<RelatorioDesempenhoTurma>()
            .HasIndex(r => r.TurmaId)
            .IsUnique();

        modelBuilder.Entity<RelatorioFrequencia>()
            .HasIndex(r => r.TurmaId)
            .IsUnique();

        // Configuração de índices
        modelBuilder.Entity<Professor>()
            .HasIndex(p => p.Email)
            .IsUnique();

        modelBuilder.Entity<Aluno>()
            .HasIndex(a => a.Matricula)
            .IsUnique();

        // Configuração de valores padrão
        modelBuilder.Entity<PlanejamentoAula>()
            .Property(p => p.CriadoEm)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<MaterialDidatico>()
            .Property(m => m.DataUpload)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<AnotacaoPlanejamento>()
            .Property(a => a.DataRegistro)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<NotaAvaliacao>()
            .Property(n => n.DataLancamento)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<AnotacaoAula>()
            .Property(a => a.DataRegistro)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<RelatorioFrequencia>()
            .Property(r => r.GeradoEm)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Configuração de cascata
        modelBuilder.Entity<MaterialDidatico>()
            .HasOne(m => m.PlanejamentoAula)
            .WithMany(p => p.MateriaisDidaticos)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnotacaoPlanejamento>()
            .HasOne(a => a.PlanejamentoAula)
            .WithMany(p => p.AnotacoesPlanejamento)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EtiquetaPlanejamento>()
            .HasOne(e => e.PlanejamentoAula)
            .WithMany(p => p.EtiquetasPlanejamento)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuestaoObjetiva>()
            .HasOne(q => q.Avaliacao)
            .WithMany(a => a.QuestoesObjetivas)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GabaritoQuestao>()
            .HasOne(g => g.QuestaoObjetiva)
            .WithOne(q => q.GabaritoQuestao)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnotacaoAula>()
            .HasOne(a => a.Aula)
            .WithOne(a => a.AnotacaoAula)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LinhaRelatorioDesempenho>()
            .HasOne(l => l.RelatorioDesempenhoTurma)
            .WithMany(r => r.LinhasRelatorioDesempenho)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LinhaRelatorioFrequencia>()
            .HasOne(l => l.RelatorioFrequencia)
            .WithMany(r => r.LinhasRelatorioFrequencia)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureColumnNames(ModelBuilder modelBuilder)
    {
        // Professor
        modelBuilder.Entity<Professor>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome).HasColumnName("nome");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.SenhaHash).HasColumnName("senhaHash");
            entity.Property(e => e.UltimoLogin).HasColumnName("ultimoLogin");
        });

        // Disciplina
        modelBuilder.Entity<Disciplina>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome).HasColumnName("nome");
        });

        // Turma
        modelBuilder.Entity<Turma>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome).HasColumnName("nome");
            entity.Property(e => e.AnoLetivo).HasColumnName("anoLetivo");
            entity.Property(e => e.Semestre).HasColumnName("semestre");
            entity.Property(e => e.Turno).HasColumnName("turno");
            entity.Property(e => e.QtdAlunos).HasColumnName("qtdAlunos");
            entity.Property(e => e.ProfessorId).HasColumnName("professorId");
            entity.Property(e => e.DisciplinaId).HasColumnName("disciplinaId");
        });

        // Aluno
        modelBuilder.Entity<Aluno>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Matricula).HasColumnName("matricula");
            entity.Property(e => e.Nome).HasColumnName("nome");
            entity.Property(e => e.TurmaId).HasColumnName("turmaId");
        });

        // Aula
        modelBuilder.Entity<Aula>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Periodo).HasColumnName("periodo");
            entity.Property(e => e.TurmaId).HasColumnName("turmaId");
        });

        // PlanejamentoAula
        modelBuilder.Entity<PlanejamentoAula>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Titulo).HasColumnName("titulo");
            entity.Property(e => e.DataAula).HasColumnName("dataAula");
            entity.Property(e => e.Objetivos).HasColumnName("objetivos");
            entity.Property(e => e.Conteudo).HasColumnName("conteudo");
            entity.Property(e => e.Metodologia).HasColumnName("metodologia");
            entity.Property(e => e.Favorito).HasColumnName("favorito");
            entity.Property(e => e.CriadoEm).HasColumnName("criadoEm");
            entity.Property(e => e.DisciplinaId).HasColumnName("disciplinaId");
        });

        // MaterialDidatico
        modelBuilder.Entity<MaterialDidatico>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NomeArquivo).HasColumnName("nomeArquivo");
            entity.Property(e => e.TipoMime).HasColumnName("tipoMime");
            entity.Property(e => e.TamanhoMB).HasColumnName("tamanhoMB");
            entity.Property(e => e.UrlArquivo).HasColumnName("urlArquivo");
            entity.Property(e => e.DataUpload).HasColumnName("dataUpload");
            entity.Property(e => e.PlanejamentoAulaId).HasColumnName("planejamentoAulaId");
        });

        // AnotacaoPlanejamento
        modelBuilder.Entity<AnotacaoPlanejamento>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Texto).HasColumnName("texto");
            entity.Property(e => e.DataRegistro).HasColumnName("dataRegistro");
            entity.Property(e => e.PlanejamentoAulaId).HasColumnName("planejamentoAulaId");
        });

        // EtiquetaPlanejamento
        modelBuilder.Entity<EtiquetaPlanejamento>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome).HasColumnName("nome");
            entity.Property(e => e.PlanejamentoAulaId).HasColumnName("planejamentoAulaId");
        });

        // Avaliacao
        modelBuilder.Entity<Avaliacao>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Titulo).HasColumnName("titulo");
            entity.Property(e => e.Tipo).HasColumnName("tipo");
            entity.Property(e => e.DataAplicacao).HasColumnName("dataAplicacao");
            entity.Property(e => e.ValorMaximo).HasColumnName("valorMaximo");
            entity.Property(e => e.DisciplinaId).HasColumnName("disciplinaId");
        });

        // QuestaoObjetiva
        modelBuilder.Entity<QuestaoObjetiva>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Numero).HasColumnName("numero");
            entity.Property(e => e.Enunciado).HasColumnName("enunciado");
            entity.Property(e => e.Valor).HasColumnName("valor");
            entity.Property(e => e.AvaliacaoId).HasColumnName("avaliacaoId");
        });

        // GabaritoQuestao
        modelBuilder.Entity<GabaritoQuestao>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlternativaCorreta).HasColumnName("alternativaCorreta");
            entity.Property(e => e.QuestaoObjetivaId).HasColumnName("questaoObjetivaId");
        });

        // NotaAvaliacao
        modelBuilder.Entity<NotaAvaliacao>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Valor).HasColumnName("valor");
            entity.Property(e => e.DataLancamento).HasColumnName("dataLancamento");
            entity.Property(e => e.Origem).HasColumnName("origem");
            entity.Property(e => e.AlunoId).HasColumnName("alunoId");
            entity.Property(e => e.AvaliacaoId).HasColumnName("avaliacaoId");
        });

        // RespostaAluno
        modelBuilder.Entity<RespostaAluno>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlternativaMarcada).HasColumnName("alternativaMarcada");
            entity.Property(e => e.Correta).HasColumnName("correta");
            entity.Property(e => e.AlunoId).HasColumnName("alunoId");
            entity.Property(e => e.QuestaoObjetivaId).HasColumnName("questaoObjetivaId");
        });

        // Frequencia
        modelBuilder.Entity<Frequencia>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.AlunoId).HasColumnName("alunoId");
            entity.Property(e => e.AulaId).HasColumnName("aulaId");
        });

        // AnotacaoAula
        modelBuilder.Entity<AnotacaoAula>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Texto).HasColumnName("texto");
            entity.Property(e => e.DataRegistro).HasColumnName("dataRegistro");
            entity.Property(e => e.AulaId).HasColumnName("aulaId");
        });

        // RelatorioDesempenhoTurma
        modelBuilder.Entity<RelatorioDesempenhoTurma>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Periodo).HasColumnName("periodo");
            entity.Property(e => e.MediaGeral).HasColumnName("mediaGeral");
            entity.Property(e => e.Mediana).HasColumnName("mediana");
            entity.Property(e => e.QtdAcimaMedia).HasColumnName("qtdAcimaMedia");
            entity.Property(e => e.QtdAbaixoMedia).HasColumnName("qtdAbaixoMedia");
            entity.Property(e => e.TurmaId).HasColumnName("turmaId");
        });

        // LinhaRelatorioDesempenho
        modelBuilder.Entity<LinhaRelatorioDesempenho>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MediaAluno).HasColumnName("mediaAluno");
            entity.Property(e => e.RelatorioDesempenhoTurmaId).HasColumnName("relatorioDesempenhoTurmaId");
            entity.Property(e => e.AlunoId).HasColumnName("alunoId");
        });

        // RelatorioFrequencia
        modelBuilder.Entity<RelatorioFrequencia>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataInicio).HasColumnName("dataInicio");
            entity.Property(e => e.DataFim).HasColumnName("dataFim");
            entity.Property(e => e.GeradoEm).HasColumnName("geradoEm");
            entity.Property(e => e.TurmaId).HasColumnName("turmaId");
        });

        // LinhaRelatorioFrequencia
        modelBuilder.Entity<LinhaRelatorioFrequencia>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TotalPresencas).HasColumnName("totalPresencas");
            entity.Property(e => e.TotalFaltas).HasColumnName("totalFaltas");
            entity.Property(e => e.PercentualAssiduidade).HasColumnName("percentualAssiduidade");
            entity.Property(e => e.RelatorioFrequenciaId).HasColumnName("relatorioFrequenciaId");
            entity.Property(e => e.AlunoId).HasColumnName("alunoId");
        });

        // Log
        modelBuilder.Entity<Log>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nivel).HasColumnName("nivel");
            entity.Property(e => e.Mensagem).HasColumnName("mensagem");
            entity.Property(e => e.Excecao).HasColumnName("excecao");
            entity.Property(e => e.StackTrace).HasColumnName("stackTrace");
            entity.Property(e => e.Usuario).HasColumnName("usuario");
            entity.Property(e => e.Endpoint).HasColumnName("endpoint");
            entity.Property(e => e.MetodoHttp).HasColumnName("metodoHttp");
            entity.Property(e => e.DataHora).HasColumnName("dataHora");
            entity.Property(e => e.IpAddress).HasColumnName("ipAddress");
            entity.Property(e => e.UserAgent).HasColumnName("userAgent");
        });
    }
}

