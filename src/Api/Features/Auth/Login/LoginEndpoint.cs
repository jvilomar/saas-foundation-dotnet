namespace SaaS.Api.Features.Auth.Login;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", LoginHandler.HandleAsync)
            .WithName("Login")
            .WithTags("Auth")
            .AllowAnonymous();

        return app;
    }
}
