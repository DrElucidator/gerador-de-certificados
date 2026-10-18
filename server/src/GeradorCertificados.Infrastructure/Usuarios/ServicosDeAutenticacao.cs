using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GeradorCertificados.Application.Usuarios;
using GeradorCertificados.Domain.Usuarios;
using GeradorCertificados.Infrastructure.Persistencia;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GeradorCertificados.Infrastructure.Usuarios;

public sealed class UsuarioRepository(AplicacaoDbContext db) : IUsuarioRepository
{
    public Task<bool> ExisteEmailAsync(string email, CancellationToken ct)
    {
        return db.Usuarios.AnyAsync(x => x.Email == email, ct);
    }

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct)
    {
        return db.Usuarios.SingleOrDefaultAsync(x => x.Email == email, ct);
    }

    public async Task AdicionarAsync(Usuario usuario, CancellationToken ct)
    {
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync(ct);
    }
}

public sealed class SenhaService : ISenhaService
{
    private static readonly object referencia = new();
    private readonly PasswordHasher<object> hasher = new();

    public string GerarHash(string senha)
    {
        return hasher.HashPassword(referencia, senha);
    }

    public bool Verificar(string senhaHash, string senha)
    {
        return hasher.VerifyHashedPassword(referencia, senhaHash, senha)
            != PasswordVerificationResult.Failed;
    }
}

public sealed class JwtSettings
{
    public string Issuer { get; init; } = "GeradorCertificados";
    public string Audience { get; init; } = "GeradorCertificados";
    public string SigningKey { get; init; } = null!;
    public int ExpiresInMinutes { get; init; } = 60;
}

public sealed class JwtTokenService(IConfiguration config) : ITokenService
{
    public TokenDeAcesso Gerar(Usuario usuario)
    {
        var settings = config.GetRequiredSection("Jwt").Get<JwtSettings>()!;
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey));
        var dataExpiracaoEmUtc = DateTimeOffset.UtcNow.AddMinutes(settings.ExpiresInMinutes);
        var token = new JwtSecurityToken(
            settings.Issuer,
            settings.Audience,
            [
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
            ],
            expires: dataExpiracaoEmUtc.UtcDateTime,
            signingCredentials: new SigningCredentials(chave, SecurityAlgorithms.HmacSha256));

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenDeAcesso(usuario.Id, accessToken, dataExpiracaoEmUtc);
    }
}
