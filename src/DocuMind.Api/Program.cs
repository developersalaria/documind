using DocuMind.Api.Endpoints;
using DocuMind.Application;
using DocuMind.Application.Documents;
using DocuMind.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

builder.Services.AddApplication(
    builder.Configuration.GetSection(ChunkingOptions.SectionName).Get<ChunkingOptions>());
builder.Services.AddInfrastructure(builder.Configuration);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.MapHealthChecks("/health");
app.MapDocumentEndpoints();
app.MapChatEndpoints();

app.Run();
