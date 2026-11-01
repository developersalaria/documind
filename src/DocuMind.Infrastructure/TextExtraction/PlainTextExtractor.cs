using System.Text;
using DocuMind.Application.Abstractions;
using DocuMind.Application.Documents;

namespace DocuMind.Infrastructure.TextExtraction;

/// <summary>
/// Reads .txt and .md files as a single page of text.
/// </summary>
public sealed class PlainTextExtractor : ITextExtractor
{
    private static readonly string[] SupportedExtensions = [".txt", ".md"];

    public bool CanExtract(string fileName) =>
        SupportedExtensions.Contains(Path.GetExtension(fileName), StringComparer.OrdinalIgnoreCase);

    public async Task<IReadOnlyList<ExtractedPage>> ExtractAsync(
        Stream content,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);

        using var reader = new StreamReader(content, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        var text = await reader.ReadToEndAsync(cancellationToken);

        return [new ExtractedPage(1, text)];
    }
}
