namespace Domain.Configuration;

public class RateLimitSettings
{
    public int PermitLimit { get; set; } = 60;
    public int WindowInSeconds { get; set; } = 60;
    public bool Apply { get; set; } = true;
}