using GeradorCertificados.Application.Usuarios;
using GeradorCertificados.Domain.Usuarios;
using GeradorCertificados.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

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
