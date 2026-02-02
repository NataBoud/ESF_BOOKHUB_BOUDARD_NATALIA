using System.Net.Http;
using System.Net.Http.Json;
using BookHub.Shared.DTOs;

namespace BookHub.BlazorClient.Services;

public class LoanService : ILoanService
{
    private readonly HttpClient _httpClient;

    public LoanService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<LoanDto>> GetUserLoansAsync(Guid userId)
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<LoanDto>>($"api/loans/user/{userId}");
        return response ?? Enumerable.Empty<LoanDto>();
    }

    public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<LoanDto>>("api/loans/overdue");
        return response ?? Enumerable.Empty<LoanDto>();
    }

    public async Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/loans", createLoanDto);

        if (response.IsSuccessStatusCode)
        {
            var loan = await response.Content.ReadFromJsonAsync<LoanDto>();
            if (loan == null)
                throw new InvalidOperationException("Impossible de créer le prêt.");

            return loan;
        }
        else
        {
            // Lire le message renvoyé par api loan service
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new ApplicationException(errorMessage); // <-- ici -> "Le livre 'X' est déjà emprunté."
        }
    }

    public async Task<LoanDto?> ReturnLoanAsync(Guid loanId)
    {
        var response = await _httpClient.PutAsync($"api/loans/{loanId}/return", null);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<LoanDto>();
    }

    // Méthode pratique pour récupérer l'historique des prêts d'un livre (admin uniquement)
    public async Task<IEnumerable<LoanDto>> GetLoansByBookIdAsync(Guid bookId)
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<LoanDto>>($"api/loans/book/{bookId}");
        return response ?? Enumerable.Empty<LoanDto>();
    }
}
