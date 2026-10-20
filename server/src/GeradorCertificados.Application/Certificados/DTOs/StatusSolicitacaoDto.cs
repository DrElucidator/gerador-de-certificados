using GeradorCertificados.Domain.Certificados;

namespace GeradorCertificados.Application.Certificados.DTOs;

public sealed record StatusSolicitacaoDto(
    Guid SolicitacaoId,
    Guid CursoId,
    StatusSolicitacaoCertificado Status,
    DateTimeOffset DataSolicitacao,
    int QuantidadeTotal,
    int QuantidadeGerada,
    int QuantidadeFalha);
