using System.Net;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Utils;

public class TokenManagerMiddleware : IMiddleware
{
    private readonly ITokenService _tokenService;

    public TokenManagerMiddleware(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (await _tokenService.IsCurrentTokenActive())
        {
            await next(context);
            return;
        }

        context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
    }
}