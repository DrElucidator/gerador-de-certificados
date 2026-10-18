namespace GeradorCertificados.Api.Contracts.Auth;
public sealed record CadastroRequest(string Email, string Senha);
public sealed record LoginRequest(string Email, string Senha);
public sealed record UsuarioResponse(Guid Id, string Email);
public sealed record TokenResponse(Guid UsuarioId, string AccessToken, DateTimeOffset DataExpiracaoEmUtc);
