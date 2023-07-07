namespace LapkaBackend.Domain.Consts;

public static class RefreshTokenConsts
{
    public static DateTime GetExpDate()
    {
        return DateTime.UtcNow.AddDays(7);
    }
}

public static class AccessTokenConsts
{
    public static DateTime GetExpDate()
    {
        return DateTime.UtcNow.AddSeconds(10);
    }
}