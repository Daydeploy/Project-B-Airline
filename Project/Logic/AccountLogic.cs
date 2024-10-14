using System;
using System.Collections.Generic;
using System.Linq;

public class AccountsLogic
{
    public List<AccountModel> _accounts;

    static public AccountModel? CurrentAccount { get; private set; }

    public AccountsLogic()
    {
        _accounts = AccountsAccess.LoadAll();
    }

    public void UpdateList(AccountModel acc)
    {
        int index = _accounts.FindIndex(s => s.Id == acc.Id);

        if (index != -1)
        {
            _accounts[index] = acc;
        }
        else
        {
            _accounts.Add(acc);
        }
        AccountsAccess.WriteAll(_accounts);
    }

    public AccountModel GetById(int id)
    {
        return _accounts.Find(i => i.Id == id);
    }

    public AccountModel CheckLogin(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }
        CurrentAccount = _accounts.Find(i => i.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase) && i.Password == password);
        return CurrentAccount;
    }
}