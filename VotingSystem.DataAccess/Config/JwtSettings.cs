namespace VotingSystem.DataAccess.Config;

public record JwtSettings
{
    public required string Secret { get; set; } = string.Empty;
    public required string Audience { get; set; } = string.Empty;
    public required string Issuer { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; }
}