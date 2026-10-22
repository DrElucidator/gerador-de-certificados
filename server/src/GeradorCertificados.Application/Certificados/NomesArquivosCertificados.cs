namespace GeradorCertificados.Application.Certificados;

public static class NomesArquivosCertificados
{
    public static string ParaZip(string nomeCurso)
        => $"Certificados - {Sanitizar(nomeCurso)}.zip";

    public static string ParaPdf(string nomeAluno, string nomeCurso)
        => $"Certificado - {Sanitizar(nomeAluno)} - {Sanitizar(nomeCurso)}.pdf";

    private static string Sanitizar(string nome)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);

        var caracteresInvalidos = Path.GetInvalidFileNameChars()
            .Concat(['<', '>', ':', '"', '/', '\\', '|', '?', '*'])
            .ToHashSet();
        var valor = new string(nome
            .Trim()
            .Select(caractere => caracteresInvalidos.Contains(caractere) ? '-' : caractere)
            .ToArray());

        return string.Join(' ', valor.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
