using DocuMind.Application.Documents;

namespace DocuMind.Application.Abstractions;

/// <summary>
/// Extracts plain text, page by page, from an uploaded file.
/// </summary>
public interface ITextExtractor
{
    /// <summary>Returns true when this extractor supports the file, based on its extension.</summary>
    bool CanExtract(string fileName);

    Task<IReadOnlyList<ExtractedPage>> ExtractAsync(Stream content, CancellationToken cancellationToken = default);
}
