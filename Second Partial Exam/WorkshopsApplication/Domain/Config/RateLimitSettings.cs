namespace Domain.Config;

public class RateLimitSettings
{
    public int PermitLimit { get; set; } = 60;
    public int WindowInSeconds { get; set; } = 60;
}
