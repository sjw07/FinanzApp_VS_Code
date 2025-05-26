using CommunityToolkit.Mvvm.ComponentModel;
using FinanceApp.Data;
using FinanceApp.Models;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.ViewModels;

public partial class MonthViewModel : ObservableObject
{
    private readonly FinanzAppContext _context;
    private readonly CurrentUserService _currentUser;

    [ObservableProperty]
    private ObservableCollection<TransactionDto> items = new();

    public MonthViewModel(FinanzAppContext context, CurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task OnAppearingAsync()
    {
        var list = await _context.Transactions
            .Where(t => t.UserId == _currentUser.Id &&
                        t.Date.Year == 2025 && t.Date.Month == 5)
            .OrderBy(t => t.Date)
            .Select(t => new TransactionDto { Date = t.Date, Name = t.Name, Amount = t.Amount })
            .ToListAsync();
        Items = new ObservableCollection<TransactionDto>(list);
    }
}
