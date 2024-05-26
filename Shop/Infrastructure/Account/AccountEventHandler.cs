namespace Shop.Infrastructure.Account;

public sealed class AccountEventHandler
{
    public event Action AccountChanged;

    public void InvokeChangeEvent()
    {
        AccountChanged?.Invoke();
    }
}