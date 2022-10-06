using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using priceapp.Repositories.Interfaces;
using priceapp.Services.Interfaces;

namespace priceapp.Services.Implementation;

public class TokenService : ITokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokensRepository _tokensRepository;

    public TokenService(IHttpContextAccessor httpContextAccessor, ITokensRepository tokensRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokensRepository = tokensRepository;
    }

    public async Task<bool> IsCurrentTokenActive()
    {
        var token = GetCurrentAsync();
        return await _tokensRepository.IsJWTTokenExistsAsync(token);
    }

    public async Task DeactivateTokensForUserAsync(int userId)
    {
        await _tokensRepository.DeleteTokensForUserAsync(userId);
    }

    public async Task DeactivateTokenAsync()
    {
        var token = GetCurrentAsync();
        await _tokensRepository.DeleteTokenAsync(token);
    }

    public async Task InsertTokenAsync(int userId, string token, int expires)
    {
        await _tokensRepository.InsertTokenAsync(userId, token, expires);
    }

    private string GetCurrentAsync()
    {
        if (_httpContextAccessor.HttpContext == null) throw new NullReferenceException("HttpContext is null");
        var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["authorization"];

        return authorizationHeader == StringValues.Empty
            ? string.Empty
            : authorizationHeader.Single().Split(" ").Last();
    }
}