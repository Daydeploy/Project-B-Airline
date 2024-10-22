namespace Testing;

[TestClass]
public class TestAccountLogic
{
    private AccountsLogic _accountsLogic;
    private List<AccountModel> _originalAccounts;


    [TestInitialize]
    public void Init()
    {
        _accountsLogic = new AccountsLogic();
        _originalAccounts = new List<AccountModel>(_accountsLogic._accounts);
    }

    [TestMethod]
    [DataRow(null, "password")]
    [DataRow("email", null)]
    [DataRow(null, null)]
    public void CheckLogin_NullValues_ReturnNull(string email, string password)
    {
        AccountsLogic ac = new AccountsLogic();
        AccountModel actual = ac.CheckLogin(email, password);
        Assert.IsNull(actual);
    }


    [TestMethod]
    [DataRow("test05@test.nl", "password", "test user!")]
    public void CheckDuplicateUser(string email, string password, string name)
    {

        // Create and add a new account using UpdateList()
        int Id = _accountsLogic._accounts.Max(x => x.Id) + 1;
        var newAccount = new AccountModel(Id, email, password, name);
        _accountsLogic.UpdateList(newAccount);

        // Assert how many accounts exist with this email. Expected: 1
        Assert.AreEqual(1, _accountsLogic._accounts.Count(x => x.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase)));

        // Create and add an account with the same email but different password.
        var newAccount2 = new AccountModel(Id, email, "test", name);
        _accountsLogic.UpdateList(newAccount2);

        // Assert to make sure the account password hasnt been changed.
        var account = _accountsLogic._accounts.First(x => x.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase));

        Assert.AreNotEqual(newAccount.Password, account.Password);
    }

    [TestMethod]
    public void Login_With_ValidCredentials_ReturnsAccount()
    {

    }
}