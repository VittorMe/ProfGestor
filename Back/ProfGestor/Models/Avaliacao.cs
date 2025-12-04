using ProfGestor.Models.Enums;

namespace ProfGestor.Models;

public class Avaliacao
{
    public long Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public TipoAvaliacao Tipo { get; set; }
    public DateOnly DataAplicacao { get; set; }
    public double ValorMaximo { get; set; }
    public long DisciplinaId { get; set; }

    // Relacionamentos
    public Disciplina Disciplina { get; set; } = null!;
    public ICollection<QuestaoObjetiva> QuestoesObjetivas { get; set; } = new List<QuestaoObjetiva>();
    public ICollection<NotaAvaliacao> NotasAvaliacao { get; set; } = new List<NotaAvaliacao>();
}

