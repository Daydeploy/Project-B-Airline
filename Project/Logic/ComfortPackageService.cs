public class ComfortPackageService
{
    public bool ValidatePackageAvailability(string flightClass)
    {
        var package = ComfortPackageDataAccess.GetComfortPackage(1);
        return package != null && package.AvailableIn.Contains(flightClass);
    }

    public void AddPackageToBooking(int bookingId, int packageId)
    {
        ComfortPackageDataAccess.AddComfortPackageToBooking(bookingId, packageId);
    }

    public decimal CalculatePackageCost(int packageId)
    {
        var package = ComfortPackageDataAccess.GetComfortPackage(packageId);
        return package?.Cost ?? 0;
    }
} 