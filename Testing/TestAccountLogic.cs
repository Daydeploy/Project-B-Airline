using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Testing
{
    [TestClass]
    public class TestAccountLogic
    {
        private AccountsLogic _accountsLogic;
        private List<AccountModel> _testAccounts;

        [TestInitialize]
        public void Setup()
        {
            _accountsLogic = new AccountsLogic();
            _testAccounts = new List<AccountModel>
            {
                new AccountModel(1, "Test", "Account", new DateTime(1990, 1, 1), "test.account@gmail.com", "test", new List<MilesModel>()),
                new AccountModel(2, "test2account@gmail.com", "test2", new DateTime(1999, 11, 11), "test2account@gmail.com", "Account2", new List<MilesModel>()),
                new AccountModel(82, "test@test.nl", "test2", new DateTime(1999, 11, 11), "test@test.nl", "test", new List<MilesModel>()),
                new AccountModel(83, "Darrin", "Wever", new DateTime(1998, 9, 8), "darrinwever@gmail.com", "test", new List<MilesModel>
                {
                    new MilesModel("Bronze", 0, 1784, "")
                })
            };

            // Simulate adding test accounts to the data source
            AccountsAccess.WriteAll(_testAccounts);
        }

        [TestMethod]
        public void TestCreateAccount()
        {
            var newAccount = new AccountModel(4, "Alice", "Johnson", new DateTime(1995, 3, 3), "alice.johnson@example.com", "password789", new List<MilesModel>());
            _accountsLogic.UpdateList(newAccount);

            var createdAccount = _accountsLogic.GetById(4);
            Assert.IsNotNull(createdAccount);
            Assert.AreEqual("Alice", createdAccount.FirstName);
        }

        [TestMethod]
        public void TestLogin_ValidCredentials()
        {
            var account = _accountsLogic.CheckLogin("test.account@gmail.com", "test");
            Assert.IsNotNull(account);
            Assert.AreEqual("Test", account.FirstName);
        }

        [TestMethod]
        public void TestLogin_InvalidCredentials()
        {
            var account = _accountsLogic.CheckLogin("test.account@gmail.com", "wrongpassword");
            Assert.IsNull(account);
        }

        [TestMethod]
        public void TestUpdateAccount()
        {
            var accountToUpdate = _accountsLogic.GetById(1);
            accountToUpdate.FirstName = "Johnathan";
            _accountsLogic.UpdateList(accountToUpdate);

            var updatedAccount = _accountsLogic.GetById(1);
            Assert.AreEqual("Johnathan", updatedAccount.FirstName);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test data
            AccountsAccess.WriteAll(new List<AccountModel>()); // Clear the accounts
        }
    }
}