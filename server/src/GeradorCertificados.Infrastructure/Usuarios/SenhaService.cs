using GeradorCertificados.Application.Usuarios;
using Microsoft.AspNetCore.Identity;

namespace GeradorCertificados.Infrastructure.Usuarios;

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
