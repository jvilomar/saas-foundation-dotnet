namespace SaaS.Api.Infrastructure.Documents;

public static class PdfServiceExtensions
{
    public static IServiceCollection AddPdfInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPdfService, QuestPdfService>();
        return services;
    }
}
