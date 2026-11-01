using DocuMind.Application.Abstractions;
using DocuMind.Infrastructure.TextExtraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocuMind.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSingleton<ITextExtractor, PlainTextExtractor>();

        // Roadmap step 2: services.AddSingleton<ITextExtractor, PdfTextExtractor>();  (UglyToad.PdfPig)
        // Roadmap step 3: register the DbContext (Npgsql + pgvector) with configuration.GetConnectionString("Postgres").
        // Roadmap step 3: register IEmbeddingGenerator<string, Embedding<float>> for configuration["AI:Provider"].
        // Roadmap step 5: register IChatClient for the same provider.

        return services;
    }
}
