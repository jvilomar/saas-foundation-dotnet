using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace SaaS.Api.Infrastructure.Documents;

public sealed class QuestPdfService : IPdfService
{
    public QuestPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Generate(Action<IDocumentContainer> composeDocument) =>
        Document.Create(composeDocument).GeneratePdf();
}
