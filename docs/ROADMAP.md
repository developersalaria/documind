# DocuMind roadmap

Work through these steps in order. Each one fits into one or two evenings, and each one makes a good GitHub issue, pull request and set of commits that show real progress.

> **Tip:** create one GitHub issue per step, work on a branch (for example `feature/pdf-support`), and merge it with a pull request even when you work alone. Reviewers and recruiters read that history.

## Step 1: Foundation (done)

- .NET 10 solution with clean architecture, central package management and warnings as errors
- Upload endpoint for `.txt` and `.md`, text chunking with overlap, unit tests
- React + Vite front-end, Docker Compose, GitHub Actions CI and Dependabot

## Step 2: PDF support

**Goal:** upload a PDF and keep page numbers for citations.

- Add the `UglyToad.PdfPig` package to `DocuMind.Infrastructure`.
- Create `PdfTextExtractor : ITextExtractor` that returns one `ExtractedPage` per PDF page.
- Register it in `DependencyInjection.AddInfrastructure`.
- Add a small sample PDF to the tests and write unit tests for the extractor.
- Add `.pdf` to the `accept` attribute in `web/src/App.tsx`.

## Step 3: Embeddings and storage

**Goal:** save every chunk with its embedding vector.

- Add `Npgsql.EntityFrameworkCore.PostgreSQL`, `Pgvector.EntityFrameworkCore` and `Microsoft.Extensions.AI`, plus one AI provider:
  - Ollama (free, runs locally): `OllamaSharp`
  - Azure OpenAI or OpenAI: `Azure.AI.OpenAI` and `Microsoft.Extensions.AI.OpenAI`
- Add an `Embedding` property (`Pgvector.Vector`) to `DocumentChunk`. Its size must match the model, for example 768 for `nomic-embed-text` or 1536 for `text-embedding-3-small`.
- Create `DocuMindDbContext` with `HasPostgresExtension("vector")`, an HNSW index on the embedding (`vector_cosine_ops`), and EF Core migrations.
- In `DocumentIngestionService`, create embeddings in batches with `IEmbeddingGenerator<string, Embedding<float>>`, then save the document.
- Add `GET /api/documents` to list uploaded documents.

## Step 4: Semantic search

**Goal:** find the chunks most related to a question.

- Embed the question with the same embedding generator.
- Return the 5 nearest chunks, ordered by cosine distance (`CosineDistance` from `Pgvector.EntityFrameworkCore`).
- Expose `GET /api/search?q=...` and add a request for it to `DocuMind.Api.http`.

## Step 5: Grounded answers with citations

**Goal:** `POST /api/chat/ask` returns a real answer.

- Build a prompt that lists the top chunks as numbered sources. Tell the model to answer only from those sources and to say it doesn't know otherwise.
- Call `IChatClient.GetResponseAsync`, then turn the sources it used into `Citation` objects.
- Unit-test the prompt builder.

## Step 6: Streaming chat UI

**Goal:** answers appear word by word.

- Use `IChatClient.GetStreamingResponseAsync` and stream with Server-Sent Events (`TypedResults.ServerSentEvents` in .NET 10).
- In React, read the stream, render text as it arrives, and show citations under the answer.
- Add a document list so users can choose which document to ask about.

## Step 7: Integration tests

- Add `Microsoft.AspNetCore.Mvc.Testing` and `Testcontainers.PostgreSql`, using the `pgvector/pgvector` image.
- Test upload, search and ask end to end against a real database, with a fake `IChatClient`.

## Step 8: Deploy with a live demo

- API on Azure Container Apps, web app on Azure Static Web Apps, database on Azure Database for PostgreSQL (Flexible Server) with the `vector` extension enabled.
- Deploy from GitHub Actions with OpenID Connect, so no cloud secrets are stored in the repository.
- Put the live demo link at the top of the README.

## Step 9: Sign-in and per-user documents

- Microsoft Entra ID: MSAL in React and JWT bearer authentication in the API.
- Each user sees only their own documents.

## Ideas for later

- Interactive API docs with `Scalar.AspNetCore`
- Answer-quality evaluation with `Microsoft.Extensions.AI.Evaluation`
- OpenTelemetry tracing and the .NET Aspire dashboard
- Rate limiting, background processing for large files, and Azure AI Search as another vector store
- An MCP server so AI assistants can search your documents

## Share your progress

After steps 3, 5 and 8, write a short LinkedIn or dev.to post about what you built and what you learned, with a link to this repository.
