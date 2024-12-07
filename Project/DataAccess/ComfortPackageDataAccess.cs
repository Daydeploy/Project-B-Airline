using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class ComfortPackageDataAccess
{
    private static string _filePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/comfortPackages.json"));
    private static List<ComfortPackage> _comfortPackages;

    static ComfortPackageDataAccess()
    {
        _comfortPackages = LoadComfortPackageDetails();
    }

    private static List<ComfortPackage> LoadComfortPackageDetails()
    {
        string json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<ComfortPackage>>(json);
    }

    public static ComfortPackage GetComfortPackage(int packageId)
    {
        return _comfortPackages.FirstOrDefault(p => p.Id == packageId);
    }

    public static void AddComfortPackageToBooking(int bookingId, int packageId)
    {
    }

    public static ComfortPackage GetBookingComfortDetails(int bookingId)
    {
        return null;
    }
}

public class ComfortPackage
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<string> Contents { get; set; }
    public decimal Cost { get; set; }
    public List<string> AvailableIn { get; set; }
} 