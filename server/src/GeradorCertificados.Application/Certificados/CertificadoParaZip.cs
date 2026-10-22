namespace GeradorCertificados.Application.Certificados;

public sealed record CertificadoParaZip(
    Guid Id,
    string NomeAluno,
    string NomeCurso);
