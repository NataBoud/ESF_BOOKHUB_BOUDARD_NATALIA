using Blazored.LocalStorage;
using BookHub.Shared.DTOs;

namespace BookHub.BlazorClient.Services;

/// <summary>
/// Gestion de l'état d'authentification côté client.
/// Utilise le LocalStorage pour persister le token JWT.
/// </summary>
public class AuthStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private const string TokenKey = "authToken";
    private const string UserKey = "currentUser";

    public UserDto? CurrentUser { get; private set; }
    public bool IsAuthenticated => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.Role == "Admin";
    public bool IsLibrarian => CurrentUser?.Role == "Librarian";

    public event Action? OnChange;

    public AuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>(TokenKey);
            var user = await _localStorage.GetItemAsync<UserDto>(UserKey);

            if (!string.IsNullOrEmpty(token) && user != null)
            {
                CurrentUser = user;
                NotifyStateChanged();
            }
        }
        catch
        {
            // Ignore errors during initialization
        }
    }

    public async Task SetUserAsync(UserDto user, string token)
    {
        await _localStorage.SetItemAsync(TokenKey, token);
        await _localStorage.SetItemAsync(UserKey, user);
        CurrentUser = user;
        NotifyStateChanged();
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        await _localStorage.RemoveItemAsync(UserKey);
        CurrentUser = null;
        NotifyStateChanged();
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>(TokenKey);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
