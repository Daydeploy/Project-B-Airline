public class PetServiceLogic
{
    //todo: Aaron presentation layer
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

    public (bool success, string error) AddPetToBooking(int bookingId, PetModel petDetails)
    {
        var validationResult = ValidatePetBooking(petDetails);
        if (!validationResult.success)
        {
            return validationResult;
        }

        PetDataAccess.SavePetBooking(petDetails, bookingId);
        return (true, string.Empty);
    }

    public decimal CalculatePetFees(PetModel petDetails)
    {
        return PetDataAccess.GetPetFees(petDetails.Type, petDetails.SeatingLocation);
    }

    public List<string> GetAvailablePetSeats(int bookingId)
    {
        return new List<string> { "1A", "1B", "2A" };
    }

    public (bool success, string error) ValidatePetBooking(PetModel petDetails)
    {
        if (!_petWeightLimits.ContainsKey(petDetails.Type))
        {
            return (false, $"Invalid pet type. Valid types are: {string.Join(", ", _petWeightLimits.Keys)}");
        }

        var (maxCabinWeight, maxWeight) = _petWeightLimits[petDetails.Type];

        if (petDetails.Weight > maxWeight)
        {
            return (false, $"Pet is too heavy. Maximum allowed weight is {maxWeight}kg.");
        }

        if (petDetails.Weight > maxCabinWeight)
        {
            petDetails.SeatingLocation = "Luggage Room";
        }
        else if ((petDetails.Type == "Dog" || petDetails.Type == "Cat") && string.IsNullOrEmpty(petDetails.SeatingLocation))
        {
            return (false, "Seating location must be specified for cats and dogs under cabin weight limit.");
        }
        else if (petDetails.Type == "Other")
        {
            petDetails.SeatingLocation = "Luggage Room";
        }

        return (true, string.Empty);
    }
}