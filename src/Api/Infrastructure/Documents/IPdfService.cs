using QuestPDF.Infrastructure;

namespace SaaS.Api.Infrastructure.Documents;

/// <summary>
/// Business-agnostic PDF generation. Consumers supply layout via QuestPDF document composition.
/// </summary>
public interface IPdfService
{
    byte[] Generate(Action<IDocumentContainer> composeDocument);
}
