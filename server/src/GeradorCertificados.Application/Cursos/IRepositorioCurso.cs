using GeradorCertificados.Domain.Cursos;

namespace GeradorCertificados.Application.Cursos;

public interface IRepositorioCurso
{
    Task AdicionarAsync(Curso curso, CancellationToken ct);

    Task<Curso?> ObterPorIdAsync(Guid id, CancellationToken ct);
}
