using System.Net.Http.Json;
using BookHub.Shared.DTOs;

namespace BookHub.BlazorClient.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users/login", loginDto);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        }

        return null;
    }

    public async Task<UserDto> RegisterAsync(CreateUserDto createUserDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users/register", createUserDto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<UserDto>())!;
    }
}
