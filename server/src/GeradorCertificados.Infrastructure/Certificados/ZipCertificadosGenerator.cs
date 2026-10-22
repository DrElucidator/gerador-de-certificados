using System.IO.Compression;
using GeradorCertificados.Application.Certificados;

namespace GeradorCertificados.Infrastructure.Certificados;

public sealed class ZipCertificadosGenerator(string diretorioBase) : IGeradorZipCertificados
{
    public string ObterCaminhoRelativo(Guid solicitacaoId)
        => $"certificados/{solicitacaoId}/certificados.zip";

    public string ObterCaminhoFisico(Guid solicitacaoId)
    {
        var diretorioSolicitacao = Path.Combine(Path.GetFullPath(diretorioBase), solicitacaoId.ToString());
        return Path.Combine(diretorioSolicitacao, "certificados.zip");
    }

    public bool Existe(Guid solicitacaoId)
        => File.Exists(ObterCaminhoFisico(solicitacaoId));

    public void GerarZip(Guid solicitacaoId, IEnumerable<CertificadoParaZip> certificadosGerados)
    {
        ArgumentNullException.ThrowIfNull(certificadosGerados);

        var certificados = certificadosGerados
            .GroupBy(x => x.Id)
            .Select(x => x.First())
            .OrderBy(x => x.NomeAluno)
            .ToList();

        if (certificados.Count == 0)
            throw new InvalidOperationException("Não há certificados gerados para empacotar.");

        var diretorioSolicitacao = Path.Combine(Path.GetFullPath(diretorioBase), solicitacaoId.ToString());
        Directory.CreateDirectory(diretorioSolicitacao);

        var caminhoTemporario = Path.Combine(diretorioSolicitacao, $"{Guid.NewGuid():N}.tmp");
        var caminhoDestino = Path.Combine(diretorioSolicitacao, "certificados.zip");

        using (var stream = File.Create(caminhoTemporario))
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: false))
        {
            var nomesUtilizados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var certificado in certificados)
            {
                var caminhoArquivo = Path.Combine(diretorioSolicitacao, $"{certificado.Id}.pdf");
                if (!File.Exists(caminhoArquivo))
                    continue;

                var nomeBase = NomesArquivosCertificados.ParaPdf(certificado.NomeAluno, certificado.NomeCurso);
                var nomeEntrada = ObterNomeDisponivel(nomeBase, nomesUtilizados);
                var entrada = archive.CreateEntry(nomeEntrada);
                using var entradaStream = entrada.Open();
                using var arquivo = File.OpenRead(caminhoArquivo);
                arquivo.CopyTo(entradaStream);
            }
        }

        if (File.Exists(caminhoDestino))
            File.Delete(caminhoDestino);

        File.Move(caminhoTemporario, caminhoDestino);
    }

    public byte[] LerZip(Guid solicitacaoId)
    {
        var caminho = ObterCaminhoFisico(solicitacaoId);
        return File.Exists(caminho)
            ? File.ReadAllBytes(caminho)
            : throw new FileNotFoundException("ZIP não encontrado para a solicitação.", caminho);
    }

    private static string ObterNomeDisponivel(string nome, ISet<string> nomesUtilizados)
    {
        if (nomesUtilizados.Add(nome))
            return nome;

        var nomeSemExtensao = Path.GetFileNameWithoutExtension(nome);
        var extensao = Path.GetExtension(nome);
        var indice = 2;
        string candidato;

        do
        {
            candidato = $"{nomeSemExtensao} ({indice++}){extensao}";
        } while (!nomesUtilizados.Add(candidato));

        return candidato;
    }
}
