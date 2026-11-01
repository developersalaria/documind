namespace DocuMind.Application.Documents;

/// <summary>
/// Turns an uploaded file into a document made of searchable chunks.
/// </summary>
public interface IDocumentIngestionService
{
    bool IsSupported(string fileName);

    Task<IngestionResult> IngestAsync(string fileName, Stream content, CancellationToken cancellationToken = default);
}
