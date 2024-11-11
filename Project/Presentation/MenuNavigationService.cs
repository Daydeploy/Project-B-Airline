using System;
using System.Collections.Generic;

public class MenuNavigationService
{
    public void DisplayMenu(List<string> menuOptions)
    {
        Console.Clear();
        Console.WriteLine("Please select an option:");
        for (int i = 0; i < menuOptions.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {menuOptions[i]}");
        }
        Console.WriteLine("0. Go Back");
    }

    public int HandleMenuSelection(int userInput, List<string> menuOptions)
    {
        if (userInput < 0 || userInput > menuOptions.Count)
        {
            Console.WriteLine("Invalid selection. Please try again.");
            return -1;
        }
        return userInput;
    }

    public void NavigateBack()
    {
        Console.WriteLine("Going back to the previous menu...");
        Console.ReadKey();
    }

    public void ClearScreen()
    {
        Console.Clear();
    }
} 