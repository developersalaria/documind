namespace DocuMind.Application.Chat;

/// <param name="Question">The user's question in plain English.</param>
/// <param name="DocumentId">Optional: limit the search to one document.</param>
public sealed record AskRequest(string Question, Guid? DocumentId = null);
