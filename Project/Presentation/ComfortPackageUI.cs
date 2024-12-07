using System;

public class ComfortPackageUI
{
    public void RenderComfortPackageOption()
    {
        var package = ComfortPackageDataAccess.GetComfortPackage(1);
        if (package != null)
        {
            Console.WriteLine($"Comfort Package: {package.Name}");
            Console.WriteLine("Contents:");
            foreach (var item in package.Contents)
            {
                Console.WriteLine($"- {item}");
            }
            Console.WriteLine($"Cost: {package.Cost:C}");
            Console.WriteLine("Available in: " + string.Join(", ", package.AvailableIn));
        }
    }

    public void ShowValidationErrors(string error)
    {
        Console.WriteLine($"Error: {error}");
    }
} 