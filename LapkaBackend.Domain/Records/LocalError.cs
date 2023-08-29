namespace LapkaBackend.Domain.Records;

public record LocalError(string Code, string Description, List<string> StackTrace) : Error(Code, Description);