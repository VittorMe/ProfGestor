# ProfGestor

## 📋 Descrição

O **ProfGestor** é uma aplicação web completa para gestão acadêmica desenvolvida para professores. O sistema permite que educadores gerenciem suas turmas, controlem a frequência dos alunos, planejem aulas, criem e apliquem avaliações, lancem notas e gerem relatórios de desempenho de forma eficiente e organizada.

A aplicação foi construída com arquitetura moderna, separando frontend e backend, proporcionando uma experiência de usuário intuitiva e uma API robusta e escalável.

## 🚀 Funcionalidades

### Autenticação e Segurança
- **Login e Registro**: Sistema de autenticação seguro com JWT (JSON Web Tokens)
- **Proteção de Rotas**: Acesso restrito apenas para usuários autenticados
- **Gerenciamento de Sessão**: Controle de sessão via cookies e tokens

### Dashboard
- **Visão Geral**: Painel com métricas principais (turmas ativas, total de alunos, planejamentos, avaliações)
- **Próximas Aulas**: Visualização das aulas programadas
- **Atividades Recentes**: Histórico de ações realizadas
- **Ações Rápidas**: Acesso rápido às funcionalidades principais

### Gestão de Turmas
- **Cadastro de Turmas**: Criação e gerenciamento de turmas
- **Gestão de Alunos**: Cadastro e controle de alunos por turma
- **Disciplinas**: Associação de disciplinas às turmas

### Controle de Frequência
- **Registro de Frequência**: Controle de presença/ausência dos alunos
- **Histórico de Frequência**: Acompanhamento do histórico de frequência por aluno e turma

### Planejamentos de Aula
- **Criação de Planejamentos**: Elaboração de planejamentos de aula detalhados
- **Anotações**: Sistema de anotações para planejamentos
- **Etiquetas**: Organização de planejamentos com etiquetas
- **Materiais Didáticos**: Gestão de materiais didáticos associados

### Avaliações
- **Criação de Avaliações**: Criação de diferentes tipos de avaliações
- **Gabaritos**: Definição de gabaritos para questões objetivas
- **Lançamento de Notas**: Sistema completo para lançamento e correção de notas
- **Questões Objetivas**: Suporte a questões de múltipla escolha

### Relatórios
- **Relatórios de Desempenho**: Análise de desempenho por turma e aluno
- **Relatórios de Frequência**: Relatórios detalhados de frequência
- **Exportação**: Geração de relatórios em PDF

## 🔗 Links

### Produção
<!-- Adicione aqui o link de produção quando disponível -->
🔗 **Link de Produção**: https://vitrequested.com.br/login

🔗 **Link de Produção - API**: https://vitrequested.com.br/swagger

### Testes
<!-- Adicione aqui o link da planilha de testes -->
📊 **Planilha de Testes**: https://docs.google.com/spreadsheets/d/13K_PU3NZL8ppPFGgXLCBkT7el8XQHoo7/edit?usp=sharing&ouid=110783221065718925791&rtpof=true&sd=true

📊 **Restante da Documentação**: https://drive.google.com/drive/folders/1wRNm-LvsjsN6ma_Ar-ynt4hLX2lUCpuO

## 🛠️ Tecnologias Utilizadas

### Frontend
- **React 19.2.0**: Biblioteca JavaScript para construção de interfaces
- **TypeScript 5.9.3**: Superset do JavaScript com tipagem estática
- **Vite 7.2.4**: Build tool e dev server de alta performance
- **React Router DOM 7.10.0**: Roteamento para aplicações React
- **Axios 1.13.2**: Cliente HTTP para requisições à API
- **React Toastify 11.0.5**: Biblioteca para notificações toast
- **jsPDF 4.0.0**: Geração de documentos PDF no cliente

### Backend
- **.NET 8.0**: Framework de desenvolvimento web
- **Entity Framework Core 8.0.11**: ORM para acesso a dados
- **MySQL 8.0.21**: Banco de dados relacional (via Pomelo.EntityFrameworkCore.MySql)
- **JWT Authentication**: Autenticação baseada em tokens
- **BCrypt.Net-Next 4.0.3**: Hash de senhas seguro
- **AutoMapper 12.0.1**: Mapeamento de objetos
- **Swagger/OpenAPI**: Documentação automática da API

### Ferramentas de Desenvolvimento
- **ESLint**: Linter para JavaScript/TypeScript
- **TypeScript ESLint**: Linting específico para TypeScript

## 📁 Estrutura do Projeto

```
ProfGestor/
│
├── Back/                          # Backend .NET
│   └── ProfGestor/
│       ├── Controllers/           # Controladores da API
│       │   ├── AuthController.cs
│       │   ├── TurmasController.cs
│       │   ├── AlunosController.cs
│       │   ├── FrequenciasController.cs
│       │   ├── PlanejamentosAulaController.cs
│       │   ├── AvaliacoesController.cs
│       │   ├── NotasAvaliacaoController.cs
│       │   ├── GabaritosController.cs
│       │   ├── RelatoriosController.cs
│       │   └── DashboardController.cs
│       │
│       ├── Services/              # Lógica de negócio
│       │   ├── AuthService.cs
│       │   ├── TurmaService.cs
│       │   ├── AlunoService.cs
│       │   ├── FrequenciaService.cs
│       │   ├── PlanejamentoAulaService.cs
│       │   ├── AvaliacaoService.cs
│       │   ├── NotaAvaliacaoService.cs
│       │   ├── GabaritoService.cs
│       │   ├── RelatorioService.cs
│       │   └── DashboardService.cs
│       │
│       ├── Repositories/          # Acesso a dados
│       │   ├── ProfessorRepository.cs
│       │   ├── TurmaRepository.cs
│       │   ├── AlunoRepository.cs
│       │   ├── FrequenciaRepository.cs
│       │   ├── PlanejamentoAulaRepository.cs
│       │   ├── AvaliacaoRepository.cs
│       │   ├── NotaAvaliacaoRepository.cs
│       │   └── GabaritoQuestaoRepository.cs
│       │
│       ├── Models/                # Entidades do banco de dados
│       │   ├── Professor.cs
│       │   ├── Turma.cs
│       │   ├── Aluno.cs
│       │   ├── Aula.cs
│       │   ├── Frequencia.cs
│       │   ├── PlanejamentoAula.cs
│       │   ├── Avaliacao.cs
│       │   ├── NotaAvaliacao.cs
│       │   └── GabaritoQuestao.cs
│       │
│       ├── DTOs/                  # Data Transfer Objects
│       │   ├── LoginRequest.cs
│       │   ├── LoginResponse.cs
│       │   ├── RegisterRequest.cs
│       │   ├── TurmaDTO.cs
│       │   ├── AlunoDTO.cs
│       │   ├── FrequenciaDTO.cs
│       │   ├── AvaliacaoDTO.cs
│       │   └── RelatorioDTO.cs
│       │
│       ├── Data/                  # Contexto do Entity Framework
│       │   └── ProfGestorContext.cs
│       │
│       ├── Middlewares/           # Middlewares customizados
│       │   └── GlobalExceptionHandlerMiddleware.cs
│       │
│       ├── Mappings/              # Configuração do AutoMapper
│       │   └── MappingProfile.cs
│       │
│       ├── Converters/            # Conversores JSON customizados
│       │   ├── DateOnlyJsonConverter.cs
│       │   └── CharJsonConverter.cs
│       │
│       ├── Binders/               # Model binders customizados
│       │   └── DateOnlyModelBinder.cs
│       │
│       ├── Exceptions/            # Exceções customizadas
│       │   ├── BusinessException.cs
│       │   ├── NotFoundException.cs
│       │   └── BadRequestException.cs
│       │
│       ├── Scripts/               # Scripts SQL
│       │   └── CreateDatabase.sql
│       │
│       ├── Program.cs             # Configuração principal da aplicação
│       ├── appsettings.json       # Configurações da aplicação
│       └── ProfGestor.csproj      # Arquivo do projeto
│
└── Front/                         # Frontend React
    └── ProfGestor/
        ├── public/                # Arquivos públicos estáticos
        │
        └── src/
            ├── pages/             # Páginas da aplicação
            │   ├── Login.tsx
            │   ├── Register.tsx
            │   ├── Dashboard.tsx
            │   ├── Turmas.tsx
            │   ├── Frequencia.tsx
            │   ├── Planejamentos.tsx
            │   ├── Avaliacoes.tsx
            │   ├── LancarNotas.tsx
            │   ├── DefinirGabarito.tsx
            │   └── Relatorios.tsx
            │
            ├── components/         # Componentes reutilizáveis
            │   ├── Layout/
            │   │   ├── Header.tsx
            │   │   └── Footer.tsx
            │   ├── ProtectedRoute.tsx
            │   ├── CookieWarning.tsx
            │   └── [outros componentes]
            │
            ├── services/          # Serviços de API
            │   ├── authService.ts
            │   ├── turmaService.ts
            │   ├── alunoService.ts
            │   ├── frequenciaService.ts
            │   ├── planejamentoService.ts
            │   ├── avaliacaoService.ts
            │   ├── notaService.ts
            │   ├── gabaritoService.ts
            │   └── relatorioService.ts
            │
            ├── contexts/          # Contextos React
            │   └── AuthContext.tsx
            │
            ├── hooks/             # Custom hooks
            │
            ├── types/             # Definições de tipos TypeScript
            │   └── auth.ts
            │
            ├── utils/             # Funções utilitárias
            │   ├── cookies.ts
            │   ├── dateFormatters.ts
            │   ├── pdfExporter.ts
            │   └── toast.ts
            │
            ├── constants/         # Constantes da aplicação
            │   └── navigation.ts
            │
            ├── config/            # Configurações
            │
            ├── assets/            # Recursos estáticos (imagens, etc.)
            │
            ├── App.tsx            # Componente principal
            ├── main.tsx           # Ponto de entrada
            └── App.css            # Estilos globais
```

## 🏗️ Arquitetura

A aplicação segue uma arquitetura em camadas:

1. **Frontend (React)**: Interface do usuário responsiva e interativa
2. **Backend (ASP.NET Core)**: API RESTful com autenticação JWT
3. **Banco de Dados (MySQL)**: Armazenamento persistente de dados
4. **Comunicação**: HTTP/HTTPS com JSON como formato de dados

### Padrões Utilizados
- **Repository Pattern**: Abstração do acesso a dados
- **Service Layer**: Lógica de negócio separada dos controladores
- **DTO Pattern**: Transferência de dados entre camadas
- **Dependency Injection**: Injeção de dependências nativa do .NET

## 🔐 Segurança

- Autenticação baseada em JWT (JSON Web Tokens)
- Hash de senhas com BCrypt
- Proteção de rotas no frontend
- Validação de tokens no backend
- Suporte a cookies para armazenamento seguro de tokens
- CORS configurado para segurança

## 📝 Notas Adicionais

- A aplicação requer cookies habilitados no navegador para funcionar corretamente
- O sistema possui um aviso de cookies que informa o usuário sobre a necessidade de habilitá-los
- A API está documentada via Swagger (acessível em `/swagger` quando o backend estiver rodando)

---

**Desenvolvido com ❤️ para facilitar a gestão acadêmica**
