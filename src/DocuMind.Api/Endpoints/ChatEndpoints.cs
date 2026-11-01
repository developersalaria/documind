using DocuMind.Application.Chat;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DocuMind.Api.Endpoints;

public static class ChatEndpoints
{
    public static IEndpointRouteBuilder MapChatEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/chat").WithTags("Chat");

        group.MapPost("/ask", Ask)
            .WithName("AskQuestion")
            .WithSummary("Ask a question about your documents")
            .WithDescription("Coming in roadmap steps 4 and 5: semantic search plus grounded answers with citations.");

        return app;
    }

    // Roadmap step 5: replace this stub with pgvector search + an IChatClient call that returns AskResponse.
    private static Results<Ok<AskResponse>, ProblemHttpResult> Ask(AskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return TypedResults.Problem("Please enter a question.", statusCode: StatusCodes.Status400BadRequest);
        }

        return TypedResults.Problem(
            "Question answering isn't implemented yet. See docs/ROADMAP.md, steps 4 and 5.",
            statusCode: StatusCodes.Status501NotImplemented,
            title: "Not implemented yet");
    }
}
