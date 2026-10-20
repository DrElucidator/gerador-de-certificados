using GeradorCertificados.Application.Cursos;
using GeradorCertificados.Application.Certificados.DTOs;
using GeradorCertificados.Domain.Certificados;
using MediatR;

namespace GeradorCertificados.Application.Certificados.Queries;

public sealed class ConsultarStatusSolicitacaoQueryHandler(
    IRepositorioCurso cursos,
    ISolicitacaoCertificadoRepository solicitacoes) : IRequestHandler<ConsultarStatusSolicitacaoQuery, StatusSolicitacaoDto>
{
    public async Task<StatusSolicitacaoDto> Handle(ConsultarStatusSolicitacaoQuery request, CancellationToken ct)
    {
        var curso = await cursos.ObterPorIdAsync(request.CursoId, ct);
        if (curso is null)
            throw new CursoInexistenteException(request.CursoId);

        var solicitacao = await solicitacoes.ObterMaisRecentePorCursoAsync(request.CursoId, ct);
        if (solicitacao is null)
            throw new SolicitacaoNaoEncontradaException(request.CursoId);

        return new StatusSolicitacaoDto(
            solicitacao.Id,
            solicitacao.CursoId,
            solicitacao.Status,
            solicitacao.DataSolicitacao,
            solicitacao.Certificados.Count,
            solicitacao.Certificados.Count(x => x.Status == StatusCertificado.Gerado),
            solicitacao.Certificados.Count(x => x.Status == StatusCertificado.Falha));
    }
}
