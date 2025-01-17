public static class AccountsAccess
{
    private static readonly string _filePath =
        Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/accounts.json"));

    private static readonly GenericJsonAccess<AccountModel> _accountAccess = new(_filePath);

    public static List<AccountModel> LoadAll()
    {
        return _accountAccess.LoadAll();
    }

    public static void WriteAll(List<AccountModel> accounts)
    {
        _accountAccess.WriteAll(accounts);
    }
}