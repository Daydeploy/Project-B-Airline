using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class TestsMenuNavigationService
{
    [TestMethod]
    public void NavigateMenu_ReturnsCorrectIndex_WhenEnterPressed()
    {
        var options = new[] { "Option 1", "Option 2", "Option 3" };
        var menuService = new MenuNavigationServiceHelper(new[]
        {
            ConsoleKey.DownArrow,
            ConsoleKey.Enter
        });

        int selectedIndex = menuService.NavigateMenu(options);

        Assert.AreEqual(1, selectedIndex);
    }

    [TestMethod]
    public void NavigateMenu_ReturnsMinusOne_WhenBackspacePressed()
    {
        var options = new[] { "Option 1", "Option 2", "Option 3" };
        var menuService = new MenuNavigationServiceHelper(new[]
        {
            ConsoleKey.Backspace
        });

        int selectedIndex = menuService.NavigateMenu(options);

        Assert.AreEqual(-1, selectedIndex);
    }
}