using DocuMind.Application.Documents;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DocuMind.Api.Endpoints;

public static class DocumentEndpoints
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    public static IEndpointRouteBuilder MapDocumentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documents").WithTags("Documents");

        group.MapPost("/", UploadAsync)
            .WithName("UploadDocument")
            .WithSummary("Upload a document")
            .WithDescription("Extracts the text and splits it into chunks. Supported today: .txt and .md.")
            // The SPA calls this API without cookie authentication, so antiforgery tokens aren't needed.
            .DisableAntiforgery();

        return app;
    }

    private static async Task<Results<Ok<IngestionResult>, ProblemHttpResult>> UploadAsync(
        IFormFile file,
        IDocumentIngestionService ingestion,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return TypedResults.Problem("The file is empty.", statusCode: StatusCodes.Status400BadRequest);
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return TypedResults.Problem("The file is larger than 10 MB.", statusCode: StatusCodes.Status413PayloadTooLarge);
        }

        if (!ingestion.IsSupported(file.FileName))
        {
            return TypedResults.Problem(
                $"'{Path.GetExtension(file.FileName)}' files are not supported yet.",
                statusCode: StatusCodes.Status415UnsupportedMediaType);
        }

        await using var stream = file.OpenReadStream();
        var result = await ingestion.IngestAsync(file.FileName, stream, cancellationToken);

        return TypedResults.Ok(result);
    }
}
