namespace GeradorCertificados.Application.Certificados;

public interface IGeradorZipCertificados
{
    string ObterCaminhoRelativo(Guid solicitacaoId);
    string ObterCaminhoFisico(Guid solicitacaoId);
    bool Existe(Guid solicitacaoId);
    void GerarZip(Guid solicitacaoId, IEnumerable<CertificadoParaZip> certificadosGerados);
    byte[] LerZip(Guid solicitacaoId);
}
