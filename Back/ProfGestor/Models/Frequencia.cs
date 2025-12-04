using ProfGestor.Models.Enums;

namespace ProfGestor.Models;

public class Frequencia
{
    public long Id { get; set; }
    public StatusFrequencia Status { get; set; }
    public long AlunoId { get; set; }
    public long AulaId { get; set; }

    // Relacionamentos
    public Aluno Aluno { get; set; } = null!;
    public Aula Aula { get; set; } = null!;
}

