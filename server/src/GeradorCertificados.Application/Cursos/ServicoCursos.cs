using GeradorCertificados.Domain.Cursos;

namespace GeradorCertificados.Application.Cursos;

public sealed class ServicoCursos(IRepositorioCurso cursos)
{
    public async Task<Curso> CadastrarAsync(
        string nome,
        string? descricao,
        int cargaHoraria,
        DateOnly dataConclusao,
        CancellationToken ct)
    {
        var curso = Curso.Criar(nome, descricao, cargaHoraria, dataConclusao);
        await cursos.AdicionarAsync(curso, ct);

        return curso;
    }

    public Task<IReadOnlyList<Curso>> ListarAsync(CancellationToken ct)
    {
        return cursos.ListarAsync(ct);
    }

    public Task<Curso?> ConsultarAsync(Guid id, CancellationToken ct)
    {
        return cursos.ObterPorIdAsync(id, ct);
    }
}
