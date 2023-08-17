namespace LapkaBackend.Application.Exceptions;

public class UnauthorizedException : Exception
{
    public string Code { get; }
    public UnauthorizedException(string code, string message) : base(message)
    {
        this.Code = code;
    }
}