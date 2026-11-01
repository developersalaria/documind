namespace DocuMind.Application.Chat;

public sealed record Citation(Guid DocumentId, string FileName, int PageNumber, string Snippet);
