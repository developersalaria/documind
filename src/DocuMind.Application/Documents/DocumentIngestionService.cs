using DocuMind.Application.Abstractions;
using DocuMind.Domain.Documents;

namespace DocuMind.Application.Documents;

public sealed class DocumentIngestionService(
    IEnumerable<ITextExtractor> extractors,
    TextChunker chunker,
    TimeProvider timeProvider) : IDocumentIngestionService
{
    private readonly ITextExtractor[] _extractors = [.. extractors];

    public bool IsSupported(string fileName) => FindExtractor(fileName) is not null;

    public async Task<IngestionResult> IngestAsync(
        string fileName,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentNullException.ThrowIfNull(content);

        var extractor = FindExtractor(fileName) ?? throw new UnsupportedDocumentException(fileName);
        var pages = await extractor.ExtractAsync(content, cancellationToken);
        var chunks = chunker.Chunk(pages);

        var document = Document.Create(fileName, pages.Count, timeProvider.GetUtcNow());
        foreach (var chunk in chunks)
        {
            document.AddChunk(chunk.PageNumber, chunk.Text);
        }

        // Roadmap step 3: create an embedding for each chunk and save the document to PostgreSQL + pgvector.

        return new IngestionResult(document.Id, document.FileName, document.PageCount, document.Chunks.Count);
    }

    private ITextExtractor? FindExtractor(string fileName) =>
        Array.Find(_extractors, extractor => extractor.CanExtract(fileName));
}
