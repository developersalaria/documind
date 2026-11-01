namespace DocuMind.Application.Chat;

/// <param name="Answer">The answer, written only from the retrieved document chunks.</param>
/// <param name="Citations">Where each part of the answer came from.</param>
public sealed record AskResponse(string Answer, IReadOnlyList<Citation> Citations);
