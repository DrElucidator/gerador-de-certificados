using GeradorCertificados.Domain.Certificados;

namespace GeradorCertificados.Application.Certificados.DTOs;

public sealed record CertificadoResumo(
    Guid Id,
    string NomeAluno,
    StatusCertificado Status,
    DateTimeOffset? DataGeracao);
