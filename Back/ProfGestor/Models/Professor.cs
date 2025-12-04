namespace ProfGestor.Models;

public class Professor
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public DateTime? UltimoLogin { get; set; }

    // Relacionamentos
    public ICollection<Turma> Turmas { get; set; } = new List<Turma>();
}

