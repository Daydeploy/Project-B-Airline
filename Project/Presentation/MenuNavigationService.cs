public static class MenuNavigationService
{
    static public int NavigateMenu(string[] options, string title = "")
    {
        int selectedIndex = 0;

        while (true)
        {
            DisplayMenu(options, title, selectedIndex);

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : options.Length - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex < options.Length - 1) ? selectedIndex + 1 : 0;
                    break;
                case ConsoleKey.Enter:
                    return selectedIndex;
                case ConsoleKey.Backspace:
                    return -1;
            }
        }
    }

    private static void DisplayMenu(string[] options, string title, int selectedIndex)
    {
        Console.Clear();
        if (!string.IsNullOrEmpty(title))
        {
            if (title.Contains("d8888b.  .d88b.  d888888b"))
            {
                Console.WriteLine(title);
                string menuTitle = "Main Menu";
                Console.WriteLine(menuTitle);
                Console.WriteLine(new string('-', menuTitle.Length));
            }
            else
            {
                Console.WriteLine(title);
                Console.WriteLine(new string('-', title.Length));
            }
        }

        for (int i = 0; i < options.Length; i++)
        {
            if (i == selectedIndex)
            {
                Console.ForegroundColor = ConsoleColor.Cyan; // Highlight color
                Console.WriteLine($"{options[i]}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"{options[i]}");
            }
        }
    }
}