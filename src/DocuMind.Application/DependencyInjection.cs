using DocuMind.Application.Documents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DocuMind.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        ChunkingOptions? chunkingOptions = null)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton(new TextChunker(chunkingOptions ?? new ChunkingOptions()));
        services.AddScoped<IDocumentIngestionService, DocumentIngestionService>();

        return services;
    }
}
