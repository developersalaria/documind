namespace DocuMind.Domain.Documents;

/// <summary>
/// A piece of a document's text, small enough to embed and retrieve on its own.
/// </summary>
public sealed class DocumentChunk
{
    private DocumentChunk()
    {
        // Required by EF Core.
    }

    internal DocumentChunk(Guid documentId, int index, int pageNumber, string text)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        Id = Guid.CreateVersion7();
        DocumentId = documentId;
        Index = index;
        PageNumber = pageNumber;
        Text = text;
    }

    public Guid Id { get; private set; }

    public Guid DocumentId { get; private set; }

    /// <summary>Position of the chunk inside the document, starting at 0.</summary>
    public int Index { get; private set; }

    /// <summary>Page the chunk came from, starting at 1. Used for citations.</summary>
    public int PageNumber { get; private set; }

    public string Text { get; private set; } = string.Empty;

    // Roadmap step 3: add the embedding vector here (Pgvector.Vector, stored in a pgvector column).
}
