namespace LapkaBackend.Application.Exceptions;

public class BadRequestException : Exception
{
    public string Code { get; set; }
    public BadRequestException(string code, string message) : base(message)
    {
        this.Code = code;
    }
    public BadRequestException(string message) : base(message)
    {
    }
    
}