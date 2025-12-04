namespace ProfGestor.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, long id) 
        : base($"{entityName} com ID {id} não foi encontrado.")
    {
    }
}

