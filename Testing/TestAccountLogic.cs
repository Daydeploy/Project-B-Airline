namespace Testing;

[TestClass]
public class TestAccountLogic
{
    private AccountsLogic _accountsLogic;
    private List<AccountModel> _originalAccounts;
    private UserAccountService _userAccountService;


    [TestInitialize]
    public void Init()
    {
        _accountsLogic = new AccountsLogic();
        _originalAccounts = new List<AccountModel>(_accountsLogic._accounts);
        _userAccountService = new UserAccountService();
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
    [DataRow("test05@test.nl", "password", "test user")]
    public void Test_Duplicate_User_Check(string email, string password, string name)
    {

        // Create and add a new account using UpdateList()
        int Id = _accountsLogic._accounts.Max(x => x.Id) + 1;
        var newAccount = new AccountModel(Id, email, password, name);
        _accountsLogic.UpdateList(newAccount);

        // Test accounts with email. Expected: 1
        Assert.AreEqual(1, _accountsLogic._accounts.Count(x => x.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase)));

        // Create and add an account with the same email but different password.
        var newAccount2 = new AccountModel(Id, email, "test", name);
        _accountsLogic.UpdateList(newAccount2);

        // Test password hasnt been changed. Expected: False
        var account = _accountsLogic._accounts.First(x => x.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase));

        Assert.AreNotEqual(newAccount.Password, account.Password);

        // Test the CreateAccount() method to verify account creation works.
        bool result = _userAccountService.CreateAccount(email, password, name);
        var createdAccount = _userAccountService._accountsLogic._accounts.FirstOrDefault(a => a.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase));

        Assert.IsTrue(result);
        Assert.AreEqual(password, createdAccount.Password);
        Assert.AreEqual(name, createdAccount.FullName);
        Assert.AreEqual(_userAccountService.CurrentUserId, createdAccount.Id);
    }

    [TestMethod]
    [DataRow("test07@test.nl", "password", "test user")]
    public void Test_Login_Functionality(string email, string password, string name)
    {
        // Test Valid Login.
        var existingAccount = _accountsLogic._accounts.First();
        var result = _accountsLogic.CheckLogin(existingAccount.EmailAddress, existingAccount.Password);

        Assert.IsNotNull(result);
        Assert.AreEqual(existingAccount.Id, result.Id);
        Assert.AreEqual(existingAccount.EmailAddress, result.EmailAddress);
        Assert.AreEqual(existingAccount.FullName, result.FullName);

        // Test invalid password
        result = _accountsLogic.CheckLogin(existingAccount.EmailAddress, "wrongpassword");
        Assert.IsNull(result);

        // Test non-existent email
        result = _accountsLogic.CheckLogin("nonexistent@email.com", "anypassword");
        Assert.IsNull(result);

        // Test case-insensitive email
        result = _accountsLogic.CheckLogin(existingAccount.EmailAddress.ToUpper(), existingAccount.Password);
        Assert.IsNotNull(result);
        Assert.AreEqual(existingAccount.Id, result.Id);

        // Verify CurrentAccount is set after successful login
        Assert.IsNotNull(AccountsLogic.CurrentAccount);
        Assert.AreEqual(existingAccount.Id, AccountsLogic.CurrentAccount.Id);

        // Test the Login() method to ensure correct handling of successful and unsuccessful logins.
        _userAccountService.CreateAccount(email, password, name);

        // Succesful login
        AccountModel loginTest = _userAccountService.Login(email, password);

        Assert.IsTrue(_userAccountService.IsLoggedIn);
        Assert.AreEqual(email, loginTest.EmailAddress);
        Assert.AreEqual(password, loginTest.Password);
        Assert.AreEqual(name, loginTest.FullName);
        Assert.AreEqual(_userAccountService.CurrentUserId, loginTest.Id);

        // Unsuccessful login wrong email and wrong password.
        AccountModel emailTest = _userAccountService.Login("email@test.nl", password);
        AccountModel passwordTest = _userAccountService.Login(email, "password1");

        Assert.IsNull(emailTest);
        Assert.IsFalse(_userAccountService.IsLoggedIn);
        Assert.IsNull(passwordTest);
        Assert.IsFalse(_userAccountService.IsLoggedIn);
    }

    [TestMethod]
    [DataRow("test06@test.nl", "password", "test user")]
    public void Test_Managing_Account(string email, string password, string name)
    {
        // Create and add a new account using UpdateList()
        int Id = _accountsLogic._accounts.Max(x => x.Id) + 1;
        var initialAccount = new AccountModel(Id, email, password, name);
        _accountsLogic.UpdateList(initialAccount);

        // Verify account was created
        var createdAccount = _accountsLogic.GetById(Id);
        Assert.IsNotNull(createdAccount);
        Assert.AreEqual(email, createdAccount.EmailAddress);
        Assert.AreEqual(password, createdAccount.Password);
        Assert.AreEqual(name, createdAccount.FullName);

        // Test modifying email. Expected: True
        var emailUpdate = new AccountModel(Id, "test07@test.nl", password, name);
        _accountsLogic.UpdateList(emailUpdate);
        var updatedEmail = _accountsLogic.GetById(Id);
        Assert.AreEqual("test07@test.nl", updatedEmail.EmailAddress);

        // Test modifying password. Expected: True
        var passwordUpdate = new AccountModel(Id, email, "password1", name);
        _accountsLogic.UpdateList(passwordUpdate);
        var updatedPassword = _accountsLogic.GetById(Id);
        Assert.AreEqual("password1", updatedPassword.Password);

        // Test modifying name. Expecte: True
        var nameUpdate = new AccountModel(Id, email, password, "test user 1");
        _accountsLogic.UpdateList(nameUpdate);
        var updatedName = _accountsLogic.GetById(Id);
        Assert.AreEqual("test user 1", updatedName.FullName);

        // Test the ManageAccount() method to confirm that account updates work as intended.

        _userAccountService.CreateAccount(email, password, name);

        int userId = _userAccountService.CurrentUserId;

        bool emailUpdateResult = _userAccountService.ManageAccount(userId, newEmail: "test02@test.nl");
        bool passwordUpdateResult = _userAccountService.ManageAccount(userId, newPassword: "newPassword");
        bool nameUpdateResult = _userAccountService.ManageAccount(userId, newFullName: "test user 2");


        Assert.IsTrue(emailUpdateResult);
        Assert.IsTrue(passwordUpdateResult);
        Assert.IsTrue(nameUpdateResult);

        var updatedAccount = _userAccountService._accountsLogic.GetById(userId);
        Assert.AreEqual("test02@test.nl", updatedAccount.EmailAddress);
        Assert.AreEqual("newPassword", updatedAccount.Password);
        Assert.AreEqual("test user 2", updatedAccount.FullName);

    }
}