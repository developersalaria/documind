namespace DocuMind.Domain.Documents;

/// <summary>
/// A document uploaded by a user and split into searchable chunks.
/// </summary>
public sealed class Document
{
    private readonly List<DocumentChunk> _chunks = [];

    private Document()
    {
        // Required by EF Core.
    }

    public Guid Id { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public int PageCount { get; private set; }

    public DateTimeOffset UploadedAt { get; private set; }

    public IReadOnlyCollection<DocumentChunk> Chunks => _chunks.AsReadOnly();

    public static Document Create(string fileName, int pageCount, DateTimeOffset uploadedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentOutOfRangeException.ThrowIfNegative(pageCount);

        return new Document
        {
            Id = Guid.CreateVersion7(),
            FileName = fileName.Trim(),
            PageCount = pageCount,
            UploadedAt = uploadedAt,
        };
    }

    public DocumentChunk AddChunk(int pageNumber, string text)
    {
        var chunk = new DocumentChunk(Id, _chunks.Count, pageNumber, text);
        _chunks.Add(chunk);
        return chunk;
    }
}
