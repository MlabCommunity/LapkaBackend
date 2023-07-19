namespace LapkaBackend.Application.Exceptions;

public class UnauthorizezdException : Exception
{
    public string Code { get; set; }
    public UnauthorizezdException(string code, string message) : base(message)
    {
        this.Code = code;
    }
}