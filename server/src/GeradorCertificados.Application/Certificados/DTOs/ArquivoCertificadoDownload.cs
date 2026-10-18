namespace GeradorCertificados.Application.Certificados.DTOs;

public sealed record ArquivoCertificadoDownload(
    byte[] Conteudo,
    string NomeArquivo,
    string ContentType);
