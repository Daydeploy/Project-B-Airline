public class PassengerModel
{
    public string Name { get; set; }
    public string SeatNumber { get; set; }
    public bool HasCheckedBaggage { get; set; }

    public PassengerModel(string name, string seatNumber, bool hasCheckedBaggage)
    {
        Name = name;
        SeatNumber = seatNumber;
        HasCheckedBaggage = hasCheckedBaggage;
    }
}