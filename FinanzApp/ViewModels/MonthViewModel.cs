using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanzApp.Data;
using FinanzApp.Models;
using Microsoft.EntityFrameworkCore;
using FinanzApp.Services;


namespace FinanzApp.ViewModels;

public partial class MonthViewModel : ObservableObject
{
    private readonly FinanzAppContext _ctx;
    private readonly ICurrentUserService _currentUser;

    public ObservableCollection<TransactionDto> Items { get; } = new();

    public MonthViewModel(FinanzAppContext ctx, ICurrentUserService currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task OnAppearingAsync()
    {
        Items.Clear();
        var items = await _ctx.Transactions
            .Where(t => t.UserId == _currentUser.CurrentUser.Id &&
                        t.Date.Year == 2025 && t.Date.Month == 5)
            .OrderBy(t => t.Date)
            .Select(t => new TransactionDto
            {
                Date = t.Date,
                Name = t.Name,
                Amount = t.Amount
            })
            .ToListAsync();

        foreach (var item in items)
            Items.Add(item);
    }
}
