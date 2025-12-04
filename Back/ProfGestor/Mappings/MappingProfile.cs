using AutoMapper;
using ProfGestor.DTOs;
using ProfGestor.Models;

namespace ProfGestor.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Professor
        CreateMap<Professor, ProfessorDTO>();
        CreateMap<ProfessorCreateDTO, Professor>()
            .ForMember(dest => dest.SenhaHash, opt => opt.Ignore())
            .ForMember(dest => dest.UltimoLogin, opt => opt.Ignore());
        CreateMap<ProfessorUpdateDTO, Professor>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SenhaHash, opt => opt.Ignore())
            .ForMember(dest => dest.UltimoLogin, opt => opt.Ignore());

        // Turma
        CreateMap<Turma, TurmaDTO>()
            .ForMember(dest => dest.ProfessorNome, opt => opt.MapFrom(src => src.Professor.Nome))
            .ForMember(dest => dest.DisciplinaNome, opt => opt.MapFrom(src => src.Disciplina.Nome))
            .ForMember(dest => dest.TotalAlunos, opt => opt.MapFrom(src => src.Alunos.Count));
        CreateMap<TurmaCreateDTO, Turma>();
        CreateMap<TurmaUpdateDTO, Turma>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // Aluno
        CreateMap<Aluno, AlunoDTO>()
            .ForMember(dest => dest.TurmaNome, opt => opt.MapFrom(src => src.Turma.Nome));
        CreateMap<AlunoCreateDTO, Aluno>();
        CreateMap<AlunoUpdateDTO, Aluno>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // PlanejamentoAula
        CreateMap<PlanejamentoAula, PlanejamentoAulaDTO>()
            .ForMember(dest => dest.DisciplinaNome, opt => opt.MapFrom(src => src.Disciplina.Nome));
        CreateMap<PlanejamentoAulaCreateDTO, PlanejamentoAula>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CriadoEm, opt => opt.Ignore());
        CreateMap<PlanejamentoAulaUpdateDTO, PlanejamentoAula>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CriadoEm, opt => opt.Ignore());

        // Log
        CreateMap<Models.Log, LogDTO>();
    }
}

