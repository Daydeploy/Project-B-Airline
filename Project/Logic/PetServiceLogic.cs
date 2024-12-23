public class PetServiceLogic
{
    private readonly Dictionary<string, (double MaxCabinWeight, double MaxWeight)> _petWeightLimits;

    public PetServiceLogic()
    {
        _petWeightLimits = new Dictionary<string, (double, double)>
        {
            { "Dog", (8.0, 32.0) },
            { "Cat", (6.0, 15.0) },
            { "Other", (4.0, 20.0) }
        };
    }

    public void AddPetToBooking(int bookingId, PetModel petDetails)
    {
        ValidatePetBooking(petDetails);
        PetDataAccess.SavePetBooking(petDetails, bookingId);
    }

    public decimal CalculatePetFees(PetModel petDetails)
    {
        return PetDataAccess.GetPetFees(petDetails.Type, petDetails.SeatingLocation);
    }

    public List<string> GetAvailablePetSeats(int bookingId)
    {
        return new List<string> { "1A", "1B", "2A" };
    }

    public void ValidatePetBooking(PetModel petDetails)
    {
        if (!_petWeightLimits.ContainsKey(petDetails.Type))
        {
            throw new ArgumentException($"Invalid pet type. Valid types are: {string.Join(", ", _petWeightLimits.Keys)}");
        }

        var (maxCabinWeight, maxWeight) = _petWeightLimits[petDetails.Type];

        if (petDetails.Weight > maxWeight)
        {
            throw new ArgumentException($"Pet is too heavy. Maximum allowed weight is {maxWeight}kg.");
        }

        if (petDetails.Weight > maxCabinWeight)
        {
            petDetails.SeatingLocation = "Luggage Room";
        }
        else if ((petDetails.Type == "Dog" || petDetails.Type == "Cat") && string.IsNullOrEmpty(petDetails.SeatingLocation))
        {
            throw new ArgumentException("Seating location must be specified for cats and dogs under cabin weight limit.");
        }
        else if (petDetails.Type == "Other")
        {
            petDetails.SeatingLocation = "Luggage Room";
        }
    }
}