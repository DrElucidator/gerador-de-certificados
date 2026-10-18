using GeradorCertificados.Domain.Certificados;

namespace GeradorCertificados.Application.Certificados.DTOs;

public sealed record SolicitacaoCertificadoResumo(
    Guid SolicitacaoId,
    Guid CursoId,
    StatusSolicitacaoCertificado Status,
    DateTimeOffset DataSolicitacao,
    int QuantidadeCertificados);
