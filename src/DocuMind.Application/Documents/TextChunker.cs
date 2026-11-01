using System.Text.RegularExpressions;

namespace DocuMind.Application.Documents;

/// <summary>
/// Splits text into overlapping chunks. It prefers to break at the end of a sentence,
/// then at a space, and never starts a chunk in the middle of a word.
/// </summary>
public sealed partial class TextChunker
{
    public const int MinimumChunkSize = 100;

    private readonly ChunkingOptions _options;

    public TextChunker(ChunkingOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentOutOfRangeException.ThrowIfLessThan(options.ChunkSize, MinimumChunkSize, nameof(options.ChunkSize));
        ArgumentOutOfRangeException.ThrowIfNegative(options.Overlap, nameof(options.Overlap));

        if (options.Overlap >= options.ChunkSize / 2)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Overlap must be less than half of ChunkSize.");
        }

        _options = options;
    }

    public IReadOnlyList<TextChunk> Chunk(IEnumerable<ExtractedPage> pages)
    {
        ArgumentNullException.ThrowIfNull(pages);

        var chunks = new List<TextChunk>();

        foreach (var page in pages)
        {
            foreach (var text in Split(page.Text))
            {
                chunks.Add(new TextChunk(chunks.Count, page.PageNumber, text));
            }
        }

        return chunks;
    }

    private IEnumerable<string> Split(string? text)
    {
        var normalized = Whitespace().Replace(text ?? string.Empty, " ").Trim();
        var start = 0;

        while (start < normalized.Length)
        {
            var end = Math.Min(start + _options.ChunkSize, normalized.Length);

            if (end < normalized.Length)
            {
                end = FindBreak(normalized, start, end);
            }

            var chunk = normalized[start..end].Trim();
            if (chunk.Length > 0)
            {
                yield return chunk;
            }

            if (end >= normalized.Length)
            {
                yield break;
            }

            start = NextStart(normalized, start, end);
        }
    }

    // Prefer the last sentence end in the second half of the window, then the last space.
    private static int FindBreak(string text, int start, int end)
    {
        var minimum = start + ((end - start) / 2);

        for (var i = end - 1; i >= minimum; i--)
        {
            if ((text[i] is '.' or '!' or '?') && i + 1 < text.Length && text[i + 1] == ' ')
            {
                return i + 1;
            }
        }

        var lastSpace = text.LastIndexOf(' ', end - 1, end - start);
        return lastSpace > start ? lastSpace : end;
    }

    // Step back by the overlap, then move forward to the start of the next word.
    private int NextStart(string text, int previousStart, int end)
    {
        var next = end - _options.Overlap;

        if (next <= previousStart)
        {
            return end;
        }

        if (text[next - 1] != ' ')
        {
            var space = text.IndexOf(' ', next);
            next = space < 0 || space >= end ? end : space + 1;
        }

        return next;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();
}
