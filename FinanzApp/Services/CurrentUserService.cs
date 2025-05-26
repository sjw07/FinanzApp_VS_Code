using FinanzApp.Data;
using FinanzApp.Data.Models;

namespace FinanzApp.Services;

public interface ICurrentUserService
{
    User CurrentUser { get; }
}

public class DummyCurrentUserService : ICurrentUserService
{
    private readonly FinanzAppContext _ctx;
    private User? _user;

    public DummyCurrentUserService(FinanzAppContext ctx)
    {
        _ctx = ctx;
    }

    public User CurrentUser => _user ??= _ctx.Users.First();
}
