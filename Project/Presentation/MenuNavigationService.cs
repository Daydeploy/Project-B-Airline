public static class MenuNavigationService
{
    static public int NavigateMenu(string[] options, string title = "")
    {
        int selectedIndex = 0;
        Console.CursorVisible = false;

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
                case ConsoleKey.Escape:
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

    public static string AirlineLogo()
    {
        return @"
 --------------------------------------------------------------------------------------------------------------------------------------------------------
|                                                                                                                                                        |
| d8888b.  .d88b.  d888888b d888888b d88888b d8888b. d8888b.  .d8b.  .88b  d88.       .d8b.  d888888b d8888b. db      d888888b d8b   db d88888b .d8888.  |
| 88  `8D .8P  Y8. `~~88~~' `~~88~~' 88'     88  `8D 88  `8D d8' `8b 88'YbdP`88      d8' `8b   `88'   88  `8D 88        `88'   888o  88 88'     88'  YP  | 
| 88oobY' 88    88    88       88    88ooooo 88oobY' 88   88 88ooo88 88  88  88      88ooo88    88    88oobY' 88         88    88V8o 88 88ooooo `8bo.    |
| 88`8b   88    88    88       88    88~~~~~ 88`8b   88   88 88~~~88 88  88  88      88~~~88    88    88`8b   88         88    88 V8o88 88~~~~~   `Y8b.  | 
| 88 `88. `8b  d8'    88       88    88.     88 `88. 88  .8D 88   88 88  88  88      88   88   .88.   88 `88. 88booo.   .88.   88  V888 88.     db   8D  |
| 88   YD  `Y88P'     YP       YP    Y88888P 88   YD Y8888D' YP   YP YP  YP  YP      YP   YP Y888888P 88   YD Y88888P Y888888P VP   V8P Y88888P `8888Y'  |
|                                                                                                                                                        |
 --------------------------------------------------------------------------------------------------------------------------------------------------------
";
    }
}