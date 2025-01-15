public class ShopUI
{
    private readonly SmallItemsLogic smallItemsLogic;
    private readonly List<ShopItemModel> cart;
    private int currentCategoryIndex;
    private int selectedItemIndex;

    public ShopUI()
    {
        smallItemsLogic = new SmallItemsLogic();
        cart = new List<ShopItemModel>();
        currentCategoryIndex = 0;
        selectedItemIndex = 0;
    }

    public List<ShopItemModel> DisplaySmallItemsShop(int bookingId, int passengerIndex)
    {
        var categories = smallItemsLogic.FetchItemDetails().ToList();

        while (true)
        {
            DisplayCurrentCategory();
            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.RightArrow:
                    if (currentCategoryIndex < categories.Count - 1)
                    {
                        currentCategoryIndex++;
                        selectedItemIndex = 0;
                    }

                    break;

                case ConsoleKey.LeftArrow:
                    if (currentCategoryIndex > 0)
                    {
                        currentCategoryIndex--;
                        selectedItemIndex = 0;
                    }

                    break;

                case ConsoleKey.UpArrow:
                    if (selectedItemIndex > 0)
                        selectedItemIndex--;
                    break;

                case ConsoleKey.DownArrow:
                    if (selectedItemIndex < categories[currentCategoryIndex].Items.Count - 1)
                        selectedItemIndex++;
                    break;

                case ConsoleKey.Enter:
                    var selectedItem = categories[currentCategoryIndex].Items[selectedItemIndex];
                    AddToCart(selectedItem, categories[currentCategoryIndex].Category);
                    break;

                case ConsoleKey.P:
                    if (cart.Any() && CompletePurchase(bookingId, passengerIndex))
                    {
                        var purchasedItems = new List<ShopItemModel>(cart);
                        cart.Clear();
                        return purchasedItems;
                    }

                    break;

                case ConsoleKey.Escape:
                    if (ConfirmExit())
                        return new List<ShopItemModel>();
                    break;
            }

            DisplayCurrentCategory();
        }
    }

    private void DisplayCurrentCategory()
    {
        Console.Clear();
        Console.WriteLine("=== Shop Items ===");
        Console.WriteLine("\nNavigation:");
        Console.WriteLine("← → Arrow keys - Browse categories");
        Console.WriteLine("↑ ↓ Arrow keys - Select item");
        Console.WriteLine("ENTER - Add item to cart");
        Console.WriteLine("P - Complete Purchase");
        Console.WriteLine("ESC - Cancel Shopping");
        Console.WriteLine(new string('─', Console.WindowWidth - 1));

        var categories = smallItemsLogic.FetchItemDetails().ToList();
        var currentCategory = categories[currentCategoryIndex];

        Console.WriteLine($"\nCategory {currentCategoryIndex + 1} of {categories.Count}");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n{currentCategory.Category}");
        Console.WriteLine(new string('─', 50));
        Console.ResetColor();

        for (int i = 0; i < currentCategory.Items.Count; i++)
        {
            var item = currentCategory.Items[i];
            if (i == selectedItemIndex)
                Console.BackgroundColor = ConsoleColor.DarkGray;
            
            Console.WriteLine($"\n  • {item.Name,-35} {(decimal)item.Price:C}");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"    {item.Description}");
            Console.ResetColor();
        }

        DisplayCart();
    }

    private void DisplayCart()
    {
        if (!cart.Any()) return;

        Console.WriteLine("\n=== Shopping Cart ===");
        Console.WriteLine(new string('─', 50));
        foreach (var item in cart)
        {
            Console.WriteLine($"{item.Name,-35} {item.Price:C}");
        }

        Console.WriteLine(new string('─', 50));
        Console.WriteLine($"Total: {cart.Sum(i => i.Price):C}");
    }

    private bool CompletePurchase(int bookingId, int passengerIndex)
    {
        Console.Clear();
        Console.WriteLine("=== Complete Purchase ===");
        Console.WriteLine("\nShopping Cart:");
        Console.WriteLine(new string('─', 50));
        foreach (var item in cart)
        {
            Console.WriteLine($"{item.Name,-35} {item.Price:C}");
        }

        Console.WriteLine(new string('─', 50));
        Console.WriteLine($"\nTotal Amount: {cart.Sum(i => i.Price):C}");
        Console.Write("\nConfirm purchase? (Y/N): ");

        if (Console.ReadLine()?.ToUpper() == "Y")
        {
            var success = smallItemsLogic.AddItemsToPassenger(cart, bookingId, passengerIndex);
            if (success)
            {
                Console.WriteLine("\nPurchase completed successfully!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return true;
            }
            else
            {
                Console.WriteLine("\nError: Failed to add items to passenger.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return false;
            }
        }

        return false;
    }

    private void AddToCart(ItemDetailModel item, string category)
    {
        cart.Add(new ShopItemModel
        {
            Name = item.Name,
            Price = (decimal)item.Price,
            Category = category,
            Description = item.Description
        });
    }


    private bool ConfirmExit()
    {
        if (!cart.Any()) return true;

        Console.Write("\nYou have items in your cart. Are you sure you want to exit? (Y/N): ");
        return Console.ReadLine()?.ToUpper() == "Y";
    }
}