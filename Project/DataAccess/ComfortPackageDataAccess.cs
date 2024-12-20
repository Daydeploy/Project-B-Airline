using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class ComfortPackageDataAccess
{
    private static string _filePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/comfortPackages.json"));
    private static GenericJsonAccess<ComfortPackageModel> _comfortPackages  = new GenericJsonAccess<ComfortPackageModel>(_filePath);
    //  private static string _filePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/flights.json"));
    // private static GenericJsonAccess<FlightModel> _flightAccess = new GenericJsonAccess<FlightModel>(_filePath);

    public static ComfortPackageModel? GetComfortPackage(int packageId)
    {
        var comfortPackageOptions = LoadAll();
        return comfortPackageOptions.Find(option => option.Id == packageId);
    }
    public static List<ComfortPackageModel> LoadAll()
    {
        return _comfortPackages.LoadAll();
    }
    // public static ComfortPackage GetBookingComfortDetails(int bookingId)
    // {
    //     return null;
    // }
}

