namespace LapkaBackend.Application.Exceptions;

public class PlaceholderException : Exception
{
    public PlaceholderException() { }
    public PlaceholderException(string message) : base(message) { }
    
    // I think there will be some kind o ExceptionHandler which will have interface to loosly couple it with other classes
    // For now lets jus use this one
}