using GeradorCertificados.Domain.Usuarios;

namespace GeradorCertificados.Application.Usuarios;

public interface IUsuarioRepository
{
    Task<bool> ExisteEmailAsync(string email, CancellationToken ct);

    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct);

    Task AdicionarAsync(Usuario usuario, CancellationToken ct);
}

public interface ISenhaService
{
    string GerarHash(string senha);

    bool Verificar(string senhaHash, string senha);
}

public interface ITokenService
{
    TokenDto Gerar(Usuario usuario);
}

public sealed record UsuarioDto(Guid Id, string Email);

public sealed record TokenDto(
    Guid UsuarioId,
    string AccessToken,
    DateTimeOffset DataExpiracaoEmUtc);

public sealed class ConflitoDeNegocioException(string message) : Exception(message);

public sealed class CredenciaisInvalidasException() : Exception("Email ou senha inválidos.");
