using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookHub.LoanService.Domain.Ports;
using BookHub.Shared.DTOs;
using BookHub.LoanService.Infrastructure.Security; 

namespace BookHub.LoanService.Infrastructure.HttpClients;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserServiceClient> _logger;
    private readonly InternalJwtTokenGenerator _tokenGenerator; // <- injection du générateur

    public UserServiceClient(HttpClient httpClient, ILogger<UserServiceClient> logger, InternalJwtTokenGenerator tokenGenerator)
    {
        _httpClient = httpClient;
        _logger = logger;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<UserDto?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Générer un JWT interne pour les appels inter-services
            var token = _tokenGenerator.Generate();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return await _httpClient.GetFromJsonAsync<UserDto>($"api/users/{userId}", cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to get user {UserId}", userId);
            return null;
        }
    }
}
