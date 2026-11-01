namespace DocuMind.Application.Documents;

/// <summary>
/// The text of one page. Plain-text files are treated as a single page.
/// </summary>
public sealed record ExtractedPage(int PageNumber, string Text);
