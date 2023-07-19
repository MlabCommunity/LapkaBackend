namespace LapkaBackend.Application.Exceptions;

public class ForbiddenExcpetion : Exception
{
    public string Code { get; set; }
    public ForbiddenExcpetion(string code, string message) : base(message)
    {
        this.Code = code;
    }
}