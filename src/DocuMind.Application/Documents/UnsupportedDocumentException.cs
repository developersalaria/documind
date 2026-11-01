namespace DocuMind.Application.Documents;

public sealed class UnsupportedDocumentException(string fileName)
    : Exception($"The file '{fileName}' is not a supported document type.")
{
    public string FileName { get; } = fileName;
}
