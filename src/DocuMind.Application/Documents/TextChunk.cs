namespace DocuMind.Application.Documents;

/// <summary>
/// A chunk of text produced by <see cref="TextChunker"/>.
/// </summary>
public sealed record TextChunk(int Index, int PageNumber, string Text);
