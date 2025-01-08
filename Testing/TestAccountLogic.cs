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
                new AccountModel(1, "Test", "Account", new DateTime(1990, 1, 1), 
                    "test.account@gmail.com", "test", "male", "NL", "1234567890", 
                    "123 Test St", null, new List<MilesModel>()),
                new AccountModel(2, "test2account@gmail.com", "test2", new DateTime(1999, 11, 11), 
                    "test2account@gmail.com", "Account2", "female", "UK", "0987654321", 
                    "456 Test Ave", null, new List<MilesModel>()),
                new AccountModel(82, "test@test.nl", "test2", new DateTime(1999, 11, 11), 
                    "test@test.nl", "test", "male", "NL", "5555555555", 
                    "789 Test Rd", null, new List<MilesModel>()),
                new AccountModel(83, "Darrin", "Wever", new DateTime(1998, 9, 8), 
                    "darrinwever@gmail.com", "test", "male", "GER", "1112223333", 
                    "321 Test Ln", null, new List<MilesModel>
                    {
                        new MilesModel("Bronze", 0, 1784, "")
                    })
            };

            AccountsAccess.WriteAll(_testAccounts);
        }

        [TestMethod]
        public void TestCreateAccount()
        {
            var newAccount = new AccountModel(4, "Jantje", "Klaasje", new DateTime(1995, 3, 3), 
                "jantje.klaasje@gmail.com", "password789", "male", "GER", "4445556666", 
                "Wijnhaven 12", null, new List<MilesModel>());
            _accountsLogic.UpdateList(newAccount);

            var createdAccount = _accountsLogic.GetById(4);
            Assert.IsNotNull(createdAccount);
            Assert.AreEqual("Jantje", createdAccount.FirstName);
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
            accountToUpdate.FirstName = "Klaas";
            _accountsLogic.UpdateList(accountToUpdate);

            var updatedAccount = _accountsLogic.GetById(1);
            Assert.AreEqual("Klaas", updatedAccount.FirstName);
        }

        [TestMethod]
        public void TestValidPassword()
        {
            Assert.IsTrue(AccountsLogic.IsValidPassword("Test123!"));
            Assert.IsFalse(AccountsLogic.IsValidPassword("nouppercaseornumber"));
            Assert.IsFalse(AccountsLogic.IsValidPassword("NoSpecialChar123"));
            Assert.IsFalse(AccountsLogic.IsValidPassword(""));
            Assert.IsFalse(AccountsLogic.IsValidPassword(null));
        }

        [TestMethod]
        public void TestValidEmail()
        {
            Assert.IsTrue(AccountsLogic.IsValidEmail("test@example.com"));
            Assert.IsFalse(AccountsLogic.IsValidEmail("invalidemail"));
            Assert.IsFalse(AccountsLogic.IsValidEmail(""));
            Assert.IsFalse(AccountsLogic.IsValidEmail(null));
        }

        [TestMethod]
        public void TestValidName()
        {
            Assert.IsTrue(AccountsLogic.IsValidName("John"));
            Assert.IsFalse(AccountsLogic.IsValidName("J"));
            Assert.IsFalse(AccountsLogic.IsValidName("ThisNameIsTooLongForValidationifimright"));
            Assert.IsFalse(AccountsLogic.IsValidName("John2"));
            Assert.IsFalse(AccountsLogic.IsValidName("John!"));
            Assert.IsFalse(AccountsLogic.IsValidName(""));
            Assert.IsFalse(AccountsLogic.IsValidName(null));
        }

        [TestMethod]
        public void TestDeleteAccount()
        {
            Assert.IsTrue(_accountsLogic.DeleteAccount(1));
            Assert.IsNull(_accountsLogic.GetById(1));

            var adminAccount = new AccountModel(
                999,
                "Admin",
                "User",
                new DateTime(1990, 1, 1),
                "admin",
                "adminpass",
                "male",
                "USA",
                "1234567890",
                "123 Admin St",
                null,
                new List<MilesModel>()
            );
            _accountsLogic.UpdateList(adminAccount);
            Assert.IsFalse(_accountsLogic.DeleteAccount(999));
        }

        [TestMethod]
        public void TestGetAllAccounts()
        {
            var accounts = _accountsLogic.GetAllAccounts();
            Assert.IsNotNull(accounts);
            Assert.IsTrue(accounts.Count > 0);
            Assert.IsFalse(accounts.Any(a => a.EmailAddress.ToLower() == "admin"));
        }

        [TestMethod]
        public void TestValidateAccountCreation()
        {
            Assert.IsTrue(_accountsLogic.ValidateAccountCreation(
                "John", "Doe", "new.email@test.com", "Test123!", 
                new DateTime(1990, 1, 1)));

            Assert.IsFalse(_accountsLogic.ValidateAccountCreation(
                "John", "Doe", "test.account@gmail.com", "Test123!", 
                new DateTime(1990, 1, 1)));

            Assert.IsFalse(_accountsLogic.ValidateAccountCreation(
                "", "Doe", "test@test.com", "Test123!", 
                new DateTime(1990, 1, 1)));
            
            Assert.IsFalse(_accountsLogic.ValidateAccountCreation(
                "John", "Doe", "test@test.com", "weak", 
                new DateTime(1990, 1, 1)));
        }

        [TestMethod]
        public void TestIsValidDateOfBirth()
        {
            Assert.IsTrue(AccountsLogic.IsValidDateOfBirth(DateTime.Now.AddYears(-25)));
            Assert.IsTrue(AccountsLogic.IsValidDateOfBirth(DateTime.Now.AddYears(-1)));
            Assert.IsFalse(AccountsLogic.IsValidDateOfBirth(DateTime.Now.AddYears(-151)));
        }

        [TestMethod]
        public void TestIsValidPhoneNumber()
        {
            Assert.IsTrue(AccountsLogic.IsValidPhoneNumber("1234567890"));
            Assert.IsTrue(AccountsLogic.IsValidPhoneNumber("123-456-7890"));
            Assert.IsFalse(AccountsLogic.IsValidPhoneNumber("123"));
            Assert.IsFalse(AccountsLogic.IsValidPhoneNumber(""));
            Assert.IsFalse(AccountsLogic.IsValidPhoneNumber(null));
        }

        [TestCleanup]
        public void Cleanup()
        {
            AccountsAccess.WriteAll(new List<AccountModel>());
        }
    }
}