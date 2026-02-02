using BookHub.Shared.DTOs;

namespace BookHub.BlazorClient.Services;

/// <summary>
/// State manager client 
/// pour partager l'état des emprunts
/// </summary>
public class LoanState
{
    private List<LoanDto> _activeLoans = new();

    public IReadOnlyList<LoanDto> ActiveLoans => _activeLoans;
    public int ActiveLoansCount => _activeLoans.Count;

    public event Action? OnChange;

    public void SetLoans(IEnumerable<LoanDto> loans)
    {
        _activeLoans = loans
            .Where(l => l.Status == LoanStatus.Active)
            .ToList();

        NotifyStateChanged();
    }

    public void AddLoan(LoanDto loan)
    {
        _activeLoans.Add(loan);
        NotifyStateChanged();
    }

    public void RemoveLoan(Guid loanId)
    {
        var loan = _activeLoans.FirstOrDefault(l => l.Id == loanId);
        if (loan != null)
        {
            _activeLoans.Remove(loan);
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
