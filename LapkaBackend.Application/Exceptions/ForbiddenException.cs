namespace LapkaBackend.Application.Exceptions;

public class ForbiddenException : Exception
{
    public string Code { get; }
    public ForbiddenException(string code, string message) : base(message)
    {
        this.Code = code;
    }
}