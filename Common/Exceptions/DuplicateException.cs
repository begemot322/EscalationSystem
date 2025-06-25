namespace Exceptions;

public class DuplicateException : Exception
{
    public DuplicateException()
        : base() { }

    public DuplicateException(string message)
        : base(message) { }

    public DuplicateException(string name, object key)
        : base($"Entity {name} with key {key} already exists.") { }
}