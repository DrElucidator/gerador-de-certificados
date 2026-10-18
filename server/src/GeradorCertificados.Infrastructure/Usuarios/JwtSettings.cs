namespace GeradorCertificados.Infrastructure.Usuarios;

public sealed class JwtSettings
{
    public string Issuer { get; init; } = "GeradorCertificados";

    public string Audience { get; init; } = "GeradorCertificados";

    public string SigningKey { get; init; } = null!;

    public int ExpiresInMinutes { get; init; } = 60;
}
