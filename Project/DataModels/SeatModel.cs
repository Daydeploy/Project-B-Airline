public class SeatModel
{
    public int FlightId { get; set; }
    public List<SeatInfo> SeatModels { get; set; }

    public SeatModel(int flightId, List<SeatInfo> seatModels)
    {
        FlightId = flightId;
        SeatModels = seatModels;
    }
}

public class SeatInfo
{
    public string SeatClass { get; set; }
    public string SeatNumber { get; set; }
    public bool IsAvailable { get; set; }
    public PassengerInfo? PassengerInfo { get; set; }

    public SeatInfo(string seatClass, string seatNumber, bool isAvailable = true, PassengerInfo? passengerInfo = null)
    {
        SeatClass = seatClass;
        SeatNumber = seatNumber;
        IsAvailable = isAvailable;
        PassengerInfo = passengerInfo;
    }
}

public class PassengerInfo
{
    public string FullName { get; set; }
    public bool HasCheckedBaggage { get; set; }
    public int CheckedBaggageAmount { get; set; }

    public PassengerInfo(string fullName, bool hasCheckedBaggage, int checkedBaggageAmount)
    {
        FullName = fullName;
        HasCheckedBaggage = hasCheckedBaggage;
        CheckedBaggageAmount = checkedBaggageAmount;
    }
}