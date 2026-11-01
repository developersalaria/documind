using DocuMind.Application.Documents;

namespace DocuMind.UnitTests.Documents;

public class TextChunkerTests
{
    private static readonly ChunkingOptions SmallChunks = new() { ChunkSize = 200, Overlap = 40 };

    [Fact]
    public void Chunk_WhitespaceOnly_ReturnsNoChunks()
    {
        var chunker = new TextChunker(SmallChunks);

        var chunks = chunker.Chunk([new ExtractedPage(1, "   \n\t  ")]);

        Assert.Empty(chunks);
    }

    [Fact]
    public void Chunk_ShortText_ReturnsOneChunkWithNormalizedWhitespace()
    {
        var chunker = new TextChunker(SmallChunks);

        var chunks = chunker.Chunk([new ExtractedPage(1, "Hello   world.\n\nThis is DocuMind.")]);

        var chunk = Assert.Single(chunks);
        Assert.Equal("Hello world. This is DocuMind.", chunk.Text);
        Assert.Equal(0, chunk.Index);
        Assert.Equal(1, chunk.PageNumber);
    }

    [Fact]
    public void Chunk_LongText_NoChunkIsLongerThanChunkSize()
    {
        var chunker = new TextChunker(SmallChunks);

        var chunks = chunker.Chunk([new ExtractedPage(1, LongText(sentences: 40))]);

        Assert.True(chunks.Count > 1);
        Assert.All(chunks, chunk => Assert.InRange(chunk.Text.Length, 1, SmallChunks.ChunkSize));
    }

    [Fact]
    public void Chunk_LongText_ConsecutiveChunksOverlap()
    {
        var chunker = new TextChunker(SmallChunks);

        var chunks = chunker.Chunk([new ExtractedPage(1, LongText(sentences: 40))]);

        for (var i = 1; i < chunks.Count; i++)
        {
            var start = chunks[i].Text[..15];
            Assert.Contains(start, chunks[i - 1].Text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Chunk_LongText_ChunksNeverStartMidWord()
    {
        var chunker = new TextChunker(SmallChunks);
        var text = LongText(sentences: 40);
        var words = text.Split(' ');

        var chunks = chunker.Chunk([new ExtractedPage(1, text)]);

        Assert.All(chunks, chunk => Assert.Contains(chunk.Text.Split(' ')[0], words));
    }

    [Fact]
    public void Chunk_MultiplePages_KeepsPageNumbersAndSequentialIndexes()
    {
        var chunker = new TextChunker(SmallChunks);

        var chunks = chunker.Chunk(
        [
            new ExtractedPage(1, "Page one text."),
            new ExtractedPage(2, "Page two text."),
        ]);

        Assert.Collection(
            chunks,
            first =>
            {
                Assert.Equal(0, first.Index);
                Assert.Equal(1, first.PageNumber);
            },
            second =>
            {
                Assert.Equal(1, second.Index);
                Assert.Equal(2, second.PageNumber);
            });
    }

    [Theory]
    [InlineData(50, 10)]    // chunk size below the minimum
    [InlineData(1000, -1)]  // negative overlap
    [InlineData(1000, 500)] // overlap must be less than half the chunk size
    public void Constructor_InvalidOptions_Throws(int chunkSize, int overlap)
    {
        var options = new ChunkingOptions { ChunkSize = chunkSize, Overlap = overlap };

        Assert.Throws<ArgumentOutOfRangeException>(() => new TextChunker(options));
    }

    private static string LongText(int sentences) =>
        string.Join(' ', Enumerable.Range(1, sentences)
            .Select(i => $"Sentence {i} explains how retrieval augmented generation works in .NET applications."));
}
