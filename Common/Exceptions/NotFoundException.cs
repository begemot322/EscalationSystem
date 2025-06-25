namespace Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException()
        : base() { }
    
    public NotFoundException(string message)
        : base(message) { }
    
    public NotFoundException(string name, object key)
        : base($"Entity {name} with key {key} was not found.") { }
}