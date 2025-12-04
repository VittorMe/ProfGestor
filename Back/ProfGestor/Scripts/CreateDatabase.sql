-- =============================================
-- Script de Criação do Banco de Dados ProfGestor
-- Baseado no Diagrama UML
-- MySQL 8.0+
-- =============================================

-- Criar o banco de dados se não existir
CREATE DATABASE IF NOT EXISTS ProfGestor 
    CHARACTER SET utf8mb4 
    COLLATE utf8mb4_unicode_ci;

USE ProfGestor;

-- =============================================
-- Limpar objetos existentes (se necessário)
-- =============================================
-- Descomente as linhas abaixo se quiser recriar o banco do zero
/*
SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS LinhaRelatorioFrequencia;
DROP TABLE IF EXISTS LinhaRelatorioDesempenho;
DROP TABLE IF EXISTS RelatorioFrequencia;
DROP TABLE IF EXISTS RelatorioDesempenhoTurma;
DROP TABLE IF EXISTS AnotacaoAula;
DROP TABLE IF EXISTS Frequencia;
DROP TABLE IF EXISTS RespostaAluno;
DROP TABLE IF EXISTS NotaAvaliacao;
DROP TABLE IF EXISTS GabaritoQuestao;
DROP TABLE IF EXISTS QuestaoObjetiva;
DROP TABLE IF EXISTS Avaliacao;
DROP TABLE IF EXISTS EtiquetaPlanejamento;
DROP TABLE IF EXISTS AnotacaoPlanejamento;
DROP TABLE IF EXISTS MaterialDidatico;
DROP TABLE IF EXISTS PlanejamentoAula;
DROP TABLE IF EXISTS Aula;
DROP TABLE IF EXISTS Aluno;
DROP TABLE IF EXISTS Disciplina;
DROP TABLE IF EXISTS Turma;
DROP TABLE IF EXISTS Professor;

SET FOREIGN_KEY_CHECKS = 1;
*/

-- =============================================
-- Tabela: Professor
-- =============================================
CREATE TABLE Professor (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    senhaHash VARCHAR(255) NOT NULL,
    ultimoLogin DATETIME NULL,
    INDEX IX_Professor_Email (email)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: Disciplina
-- =============================================
CREATE TABLE Disciplina (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: Turma
-- =============================================
CREATE TABLE Turma (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    anoLetivo INT NOT NULL,
    semestre INT NOT NULL,
    turno VARCHAR(50) NOT NULL,
    qtdAlunos INT NOT NULL DEFAULT 0,
    professorId BIGINT NOT NULL,
    disciplinaId BIGINT NOT NULL,
    CONSTRAINT FK_Turma_Professor FOREIGN KEY (professorId) REFERENCES Professor(id) ON DELETE RESTRICT,
    CONSTRAINT FK_Turma_Disciplina FOREIGN KEY (disciplinaId) REFERENCES Disciplina(id) ON DELETE RESTRICT,
    INDEX IX_Turma_ProfessorId (professorId),
    INDEX IX_Turma_DisciplinaId (disciplinaId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: Aluno
-- =============================================
CREATE TABLE Aluno (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    matricula VARCHAR(50) NOT NULL UNIQUE,
    nome VARCHAR(255) NOT NULL,
    turmaId BIGINT NOT NULL,
    CONSTRAINT FK_Aluno_Turma FOREIGN KEY (turmaId) REFERENCES Turma(id) ON DELETE RESTRICT,
    INDEX IX_Aluno_TurmaId (turmaId),
    INDEX IX_Aluno_Matricula (matricula)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: Aula
-- =============================================
CREATE TABLE Aula (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    data DATE NOT NULL,
    periodo VARCHAR(50) NOT NULL,
    turmaId BIGINT NOT NULL,
    CONSTRAINT FK_Aula_Turma FOREIGN KEY (turmaId) REFERENCES Turma(id) ON DELETE RESTRICT,
    INDEX IX_Aula_TurmaId (turmaId),
    INDEX IX_Aula_Data (data)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: PlanejamentoAula
-- =============================================
CREATE TABLE PlanejamentoAula (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    titulo VARCHAR(255) NOT NULL,
    dataAula DATE NOT NULL,
    objetivos TEXT NULL,
    conteudo LONGTEXT NULL,
    metodologia TEXT NULL,
    favorito BOOLEAN NOT NULL DEFAULT FALSE,
    criadoEm DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    disciplinaId BIGINT NOT NULL,
    CONSTRAINT FK_PlanejamentoAula_Disciplina FOREIGN KEY (disciplinaId) REFERENCES Disciplina(id) ON DELETE RESTRICT,
    INDEX IX_PlanejamentoAula_DisciplinaId (disciplinaId),
    INDEX IX_PlanejamentoAula_DataAula (dataAula),
    INDEX IX_PlanejamentoAula_Favorito (favorito)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: MaterialDidatico
-- =============================================
CREATE TABLE MaterialDidatico (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    nomeArquivo VARCHAR(255) NOT NULL,
    tipoMime VARCHAR(100) NOT NULL,
    tamanhoMB DOUBLE NOT NULL,
    urlArquivo VARCHAR(500) NOT NULL,
    dataUpload DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    planejamentoAulaId BIGINT NOT NULL,
    CONSTRAINT FK_MaterialDidatico_PlanejamentoAula FOREIGN KEY (planejamentoAulaId) REFERENCES PlanejamentoAula(id) ON DELETE CASCADE,
    INDEX IX_MaterialDidatico_PlanejamentoAulaId (planejamentoAulaId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: AnotacaoPlanejamento
-- =============================================
CREATE TABLE AnotacaoPlanejamento (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    texto TEXT NOT NULL,
    dataRegistro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    planejamentoAulaId BIGINT NOT NULL,
    CONSTRAINT FK_AnotacaoPlanejamento_PlanejamentoAula FOREIGN KEY (planejamentoAulaId) REFERENCES PlanejamentoAula(id) ON DELETE CASCADE,
    INDEX IX_AnotacaoPlanejamento_PlanejamentoAulaId (planejamentoAulaId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: EtiquetaPlanejamento
-- =============================================
CREATE TABLE EtiquetaPlanejamento (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    planejamentoAulaId BIGINT NOT NULL,
    CONSTRAINT FK_EtiquetaPlanejamento_PlanejamentoAula FOREIGN KEY (planejamentoAulaId) REFERENCES PlanejamentoAula(id) ON DELETE CASCADE,
    INDEX IX_EtiquetaPlanejamento_PlanejamentoAulaId (planejamentoAulaId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: Avaliacao
-- =============================================
CREATE TABLE Avaliacao (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    titulo VARCHAR(255) NOT NULL,
    tipo ENUM('PROVA', 'TRABALHO', 'ATIVIDADE', 'OUTRO') NOT NULL,
    dataAplicacao DATE NOT NULL,
    valorMaximo DOUBLE NOT NULL,
    disciplinaId BIGINT NOT NULL,
    CONSTRAINT FK_Avaliacao_Disciplina FOREIGN KEY (disciplinaId) REFERENCES Disciplina(id) ON DELETE RESTRICT,
    INDEX IX_Avaliacao_DisciplinaId (disciplinaId),
    INDEX IX_Avaliacao_DataAplicacao (dataAplicacao)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: QuestaoObjetiva
-- =============================================
CREATE TABLE QuestaoObjetiva (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    numero INT NOT NULL,
    enunciado TEXT NOT NULL,
    valor DOUBLE NOT NULL,
    avaliacaoId BIGINT NOT NULL,
    CONSTRAINT FK_QuestaoObjetiva_Avaliacao FOREIGN KEY (avaliacaoId) REFERENCES Avaliacao(id) ON DELETE CASCADE,
    INDEX IX_QuestaoObjetiva_AvaliacaoId (avaliacaoId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: GabaritoQuestao
-- =============================================
CREATE TABLE GabaritoQuestao (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    alternativaCorreta CHAR(1) NOT NULL,
    questaoObjetivaId BIGINT NOT NULL UNIQUE,
    CONSTRAINT FK_GabaritoQuestao_QuestaoObjetiva FOREIGN KEY (questaoObjetivaId) REFERENCES QuestaoObjetiva(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: NotaAvaliacao
-- =============================================
CREATE TABLE NotaAvaliacao (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    valor DOUBLE NOT NULL,
    dataLancamento DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    origem VARCHAR(100) NULL,
    alunoId BIGINT NOT NULL,
    avaliacaoId BIGINT NOT NULL,
    CONSTRAINT FK_NotaAvaliacao_Aluno FOREIGN KEY (alunoId) REFERENCES Aluno(id) ON DELETE RESTRICT,
    CONSTRAINT FK_NotaAvaliacao_Avaliacao FOREIGN KEY (avaliacaoId) REFERENCES Avaliacao(id) ON DELETE RESTRICT,
    INDEX IX_NotaAvaliacao_AlunoId (alunoId),
    INDEX IX_NotaAvaliacao_AvaliacaoId (avaliacaoId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: RespostaAluno
-- =============================================
CREATE TABLE RespostaAluno (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    alternativaMarcada CHAR(1) NOT NULL,
    correta BOOLEAN NOT NULL,
    alunoId BIGINT NOT NULL,
    questaoObjetivaId BIGINT NOT NULL,
    CONSTRAINT FK_RespostaAluno_Aluno FOREIGN KEY (alunoId) REFERENCES Aluno(id) ON DELETE RESTRICT,
    CONSTRAINT FK_RespostaAluno_QuestaoObjetiva FOREIGN KEY (questaoObjetivaId) REFERENCES QuestaoObjetiva(id) ON DELETE RESTRICT,
    INDEX IX_RespostaAluno_AlunoId (alunoId),
    INDEX IX_RespostaAluno_QuestaoObjetivaId (questaoObjetivaId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: Frequencia
-- =============================================
CREATE TABLE Frequencia (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    status ENUM('PRESENTE', 'FALTA', 'FALTA_JUSTIFICADA') NOT NULL,
    alunoId BIGINT NOT NULL,
    aulaId BIGINT NOT NULL,
    CONSTRAINT FK_Frequencia_Aluno FOREIGN KEY (alunoId) REFERENCES Aluno(id) ON DELETE RESTRICT,
    CONSTRAINT FK_Frequencia_Aula FOREIGN KEY (aulaId) REFERENCES Aula(id) ON DELETE RESTRICT,
    INDEX IX_Frequencia_AlunoId (alunoId),
    INDEX IX_Frequencia_AulaId (aulaId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: AnotacaoAula
-- =============================================
CREATE TABLE AnotacaoAula (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    texto TEXT NOT NULL,
    dataRegistro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    aulaId BIGINT NOT NULL UNIQUE,
    CONSTRAINT FK_AnotacaoAula_Aula FOREIGN KEY (aulaId) REFERENCES Aula(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: RelatorioDesempenhoTurma
-- =============================================
CREATE TABLE RelatorioDesempenhoTurma (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    periodo VARCHAR(50) NOT NULL,
    mediaGeral DOUBLE NOT NULL,
    mediana DOUBLE NOT NULL,
    qtdAcimaMedia INT NOT NULL DEFAULT 0,
    qtdAbaixoMedia INT NOT NULL DEFAULT 0,
    turmaId BIGINT NOT NULL UNIQUE,
    CONSTRAINT FK_RelatorioDesempenhoTurma_Turma FOREIGN KEY (turmaId) REFERENCES Turma(id) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: LinhaRelatorioDesempenho
-- =============================================
CREATE TABLE LinhaRelatorioDesempenho (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    mediaAluno DOUBLE NOT NULL,
    relatorioDesempenhoTurmaId BIGINT NOT NULL,
    alunoId BIGINT NOT NULL,
    CONSTRAINT FK_LinhaRelatorioDesempenho_RelatorioDesempenhoTurma FOREIGN KEY (relatorioDesempenhoTurmaId) REFERENCES RelatorioDesempenhoTurma(id) ON DELETE CASCADE,
    CONSTRAINT FK_LinhaRelatorioDesempenho_Aluno FOREIGN KEY (alunoId) REFERENCES Aluno(id) ON DELETE RESTRICT,
    INDEX IX_LinhaRelatorioDesempenho_RelatorioDesempenhoTurmaId (relatorioDesempenhoTurmaId),
    INDEX IX_LinhaRelatorioDesempenho_AlunoId (alunoId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: RelatorioFrequencia
-- =============================================
CREATE TABLE RelatorioFrequencia (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    dataInicio DATE NOT NULL,
    dataFim DATE NOT NULL,
    geradoEm DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    turmaId BIGINT NOT NULL UNIQUE,
    CONSTRAINT FK_RelatorioFrequencia_Turma FOREIGN KEY (turmaId) REFERENCES Turma(id) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: LinhaRelatorioFrequencia
-- =============================================
CREATE TABLE LinhaRelatorioFrequencia (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    totalPresencas INT NOT NULL DEFAULT 0,
    totalFaltas INT NOT NULL DEFAULT 0,
    percentualAssiduidade DOUBLE NOT NULL,
    relatorioFrequenciaId BIGINT NOT NULL,
    alunoId BIGINT NOT NULL,
    CONSTRAINT FK_LinhaRelatorioFrequencia_RelatorioFrequencia FOREIGN KEY (relatorioFrequenciaId) REFERENCES RelatorioFrequencia(id) ON DELETE CASCADE,
    CONSTRAINT FK_LinhaRelatorioFrequencia_Aluno FOREIGN KEY (alunoId) REFERENCES Aluno(id) ON DELETE RESTRICT,
    INDEX IX_LinhaRelatorioFrequencia_RelatorioFrequenciaId (relatorioFrequenciaId),
    INDEX IX_LinhaRelatorioFrequencia_AlunoId (alunoId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tabela: Log
-- =============================================
CREATE TABLE Log (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    nivel VARCHAR(50) NOT NULL,
    mensagem TEXT NOT NULL,
    excecao TEXT NULL,
    stackTrace TEXT NULL,
    usuario VARCHAR(255) NULL,
    endpoint VARCHAR(500) NULL,
    metodoHttp VARCHAR(10) NULL,
    dataHora DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ipAddress VARCHAR(45) NULL,
    userAgent VARCHAR(500) NULL,
    INDEX IX_Log_DataHora (dataHora),
    INDEX IX_Log_Nivel (nivel),
    INDEX IX_Log_Usuario (usuario)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Script concluído
-- =============================================
SELECT 'Banco de dados ProfGestor criado com sucesso!' AS Mensagem;
