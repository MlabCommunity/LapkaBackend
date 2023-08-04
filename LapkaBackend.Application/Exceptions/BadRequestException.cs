namespace LapkaBackend.Application.Exceptions;

public class BadRequestException : Exception
{
    public string Code { get; }
    public BadRequestException(string code, string message) : base(message)
    {
        this.Code = code;
    }

}