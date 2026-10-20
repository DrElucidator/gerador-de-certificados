using GeradorCertificados.Application.Certificados.DTOs;
using MediatR;

namespace GeradorCertificados.Application.Certificados.Queries;

public sealed record ConsultarStatusSolicitacaoQuery(Guid CursoId) : IRequest<StatusSolicitacaoDto>;
