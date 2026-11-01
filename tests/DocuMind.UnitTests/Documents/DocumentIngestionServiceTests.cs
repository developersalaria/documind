using DocuMind.Application.Abstractions;
using DocuMind.Application.Documents;

namespace DocuMind.UnitTests.Documents;

public class DocumentIngestionServiceTests
{
    [Fact]
    public async Task IngestAsync_SupportedFile_ReturnsPageAndChunkCounts()
    {
        var service = CreateService(new FakeExtractor(".txt", "First page.", "Second page."));
        using var content = new MemoryStream();

        var result = await service.IngestAsync("notes.txt", content);

        Assert.Equal("notes.txt", result.FileName);
        Assert.Equal(2, result.PageCount);
        Assert.Equal(2, result.ChunkCount);
        Assert.NotEqual(Guid.Empty, result.DocumentId);
    }

    [Fact]
    public async Task IngestAsync_UnsupportedFile_ThrowsUnsupportedDocumentException()
    {
        var service = CreateService(new FakeExtractor(".txt"));
        using var content = new MemoryStream();

        await Assert.ThrowsAsync<UnsupportedDocumentException>(() => service.IngestAsync("photo.png", content));
    }

    [Theory]
    [InlineData("notes.txt", true)]
    [InlineData("NOTES.TXT", true)]
    [InlineData("photo.png", false)]
    public void IsSupported_ChecksRegisteredExtractors(string fileName, bool expected)
    {
        var service = CreateService(new FakeExtractor(".txt"));

        Assert.Equal(expected, service.IsSupported(fileName));
    }

    private static DocumentIngestionService CreateService(params ITextExtractor[] extractors) =>
        new(extractors, new TextChunker(new ChunkingOptions()), TimeProvider.System);

    private sealed class FakeExtractor(string extension, params string[] pages) : ITextExtractor
    {
        public bool CanExtract(string fileName) =>
            string.Equals(Path.GetExtension(fileName), extension, StringComparison.OrdinalIgnoreCase);

        public Task<IReadOnlyList<ExtractedPage>> ExtractAsync(Stream content, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ExtractedPage>>(
                pages.Select((text, index) => new ExtractedPage(index + 1, text)).ToList());
    }
}
