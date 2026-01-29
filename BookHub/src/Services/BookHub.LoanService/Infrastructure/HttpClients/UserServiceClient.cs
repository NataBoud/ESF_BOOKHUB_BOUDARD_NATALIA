using System.Net.Http.Json;
using BookHub.LoanService.Domain.Ports;
using BookHub.Shared.DTOs;

namespace BookHub.LoanService.Infrastructure.HttpClients;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserServiceClient> _logger;

    public UserServiceClient(HttpClient httpClient, ILogger<UserServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UserDto?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserDto>($"api/users/{userId}", cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to get user {UserId}", userId);
            return null;
        }
    }
}
