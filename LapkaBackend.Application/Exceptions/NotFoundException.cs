namespace LapkaBackend.Application.Exceptions;

public class NotFoundException : Exception
{
    public string Code { get; set; }
    public NotFoundException(string code, string message) : base(message)
    {
        this.Code = code;
    }
}