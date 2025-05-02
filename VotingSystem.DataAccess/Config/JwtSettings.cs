namespace VotingSystem.DataAccess.Config;

public record JwtSettings
{
    public required string Secret { get; init; }
    public required string Audience { get; init; }
    public required string Issuer { get; init; }
    public int AccessTokenExpirationMinutes { get; init; }
}