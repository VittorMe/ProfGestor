namespace ProfGestor.DTOs;

public class DisciplinaDTO
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class DisciplinaCreateDTO
{
    public string Nome { get; set; } = string.Empty;
}

public class DisciplinaUpdateDTO
{
    public string Nome { get; set; } = string.Empty;
}

