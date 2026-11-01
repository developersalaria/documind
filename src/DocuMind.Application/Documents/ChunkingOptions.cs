namespace DocuMind.Application.Documents;

/// <summary>
/// Settings for <see cref="TextChunker"/>, bound from the "Chunking" configuration section.
/// </summary>
public sealed class ChunkingOptions
{
    public const string SectionName = "Chunking";

    /// <summary>Maximum number of characters in one chunk.</summary>
    public int ChunkSize { get; set; } = 1000;

    /// <summary>Characters repeated between consecutive chunks, so context isn't lost at the edges.</summary>
    public int Overlap { get; set; } = 150;
}
