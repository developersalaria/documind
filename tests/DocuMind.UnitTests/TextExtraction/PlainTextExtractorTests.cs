using System.Text;
using DocuMind.Infrastructure.TextExtraction;

namespace DocuMind.UnitTests.TextExtraction;

public class PlainTextExtractorTests
{
    private readonly PlainTextExtractor _extractor = new();

    [Theory]
    [InlineData("readme.md", true)]
    [InlineData("notes.TXT", true)]
    [InlineData("report.pdf", false)]
    [InlineData("no-extension", false)]
    public void CanExtract_UsesFileExtension(string fileName, bool expected)
    {
        Assert.Equal(expected, _extractor.CanExtract(fileName));
    }

    [Fact]
    public async Task ExtractAsync_ReturnsTheWholeFileAsOnePage()
    {
        using var content = new MemoryStream(Encoding.UTF8.GetBytes("Hello from DocuMind."));

        var pages = await _extractor.ExtractAsync(content);

        var page = Assert.Single(pages);
        Assert.Equal(1, page.PageNumber);
        Assert.Equal("Hello from DocuMind.", page.Text);
    }
}
