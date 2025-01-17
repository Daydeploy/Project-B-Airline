using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Testing
{
    [TestClass]
    public class TestAdminAccount
    {
        private UserAccountServiceLogic _userAccountService;
        private StringWriter _consoleOutput;
        private TextWriter _originalOutput;

        [TestInitialize]
        public void Setup()
        {
            // Setup test environment
            _userAccountService = new UserAccountServiceLogic();
            _consoleOutput = new StringWriter();
            _originalOutput = Console.Out;
            Console.SetOut(_consoleOutput);

            // Create test admin account
            var adminAccount = new AccountModel(
                1,
                "Admin",
                "User",
                new DateTime(1990, 1, 1),
                "admin",
                "Admin123!",
                "Other",
                "NL",
                "1234567890",
                "Test Address",
                null,
                new List<MilesModel>()
            );

            _userAccountService._accountsLogic.UpdateList(adminAccount);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Console.SetOut(_originalOutput);
            _consoleOutput.Dispose();
        }

        [TestMethod]
        public void TestAdminLogin_ValidCredentials_Success()
        {
            // Arrange
            string email = "admin";
            string password = "Admin123!";

            // Act
            var result = _userAccountService.Login(email, password);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Admin", result.FirstName);
            Assert.IsTrue(_userAccountService.IsLoggedIn);
        }

        [TestMethod]
        public void TestAdminLogin_InvalidCredentials_Fails()
        {
            // Arrange
            string email = "admin";
            string password = "wrongpassword";

            // Act
            var result = _userAccountService.Login(email, password);

            // Assert
            Assert.IsNull(result);
            Assert.IsFalse(_userAccountService.IsLoggedIn);
        }

        [TestMethod]
        public void TestAdminLogout_Success()
        {
            // Arrange
            _userAccountService.Login("admin", "Admin123!");

            // Act
            _userAccountService.Logout();

            // Assert
            Assert.IsFalse(_userAccountService.IsLoggedIn);
            Assert.AreEqual(-1, _userAccountService.CurrentUserId);
        }

        [TestMethod]
        public void TestIsAdmin_WithAdminAccount_ReturnsTrue()
        {
            // Arrange
            _userAccountService.Login("admin", "Admin123!");
            var account = _userAccountService.CurrentAccount;

            // Act
            bool isAdmin = account.EmailAddress.ToLower() == "admin";

            // Assert
            Assert.IsTrue(isAdmin);
        }

        [TestMethod]
        public void TestIsAdmin_WithRegularAccount_ReturnsFalse()
        {
            // Arrange
            var regularAccount = new AccountModel(
                2,
                "Regular",
                "User",
                new DateTime(1990, 1, 1),
                "regular@test.com",
                "Test123!",
                "Other",
                "NL",
                "1234567890",
                "Test Address",
                null,
                new List<MilesModel>()
            );
            _userAccountService._accountsLogic.UpdateList(regularAccount);
            _userAccountService.Login("regular@test.com", "Test123!");

            // Act
            var account = _userAccountService.CurrentAccount;
            bool isAdmin = account.EmailAddress.ToLower() == "admin";

            // Assert
            Assert.IsFalse(isAdmin);
        }

        [TestMethod]
        public void TestHandleSelection_AdminMenu_NavigationSuccess()
        {
            // Arrange
            _userAccountService.Login("admin", "Admin123!");
            bool exit = false;

            // Act & Assert - Test each menu option
            string[] validOptions = { "Manage Airports", "Manage Flights", "Manage Accounts", "Manage Finance", "Logout" };
            foreach (var option in validOptions)
            {
                Assert.IsTrue(ValidateMenuOption(option, ref exit));
            }
        }

        private bool ValidateMenuOption(string option, ref bool exit)
        {
            try
            {
                AdminAccountUI.HandleSelection(option, ref exit);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}