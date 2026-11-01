namespace DocuMind.Application.Documents;

public sealed record IngestionResult(Guid DocumentId, string FileName, int PageCount, int ChunkCount);
