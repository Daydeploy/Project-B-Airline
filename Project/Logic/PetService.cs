public class PetService
{
    private readonly Dictionary<string, (double MaxCabinWeight, double MaxWeight)> _petWeightLimits;

    public PetService()
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
            throw new InvalidOperationException("Invalid pet type.");
        }

        var (maxCabinWeight, maxWeight) = _petWeightLimits[petDetails.Type];

        if (petDetails.Weight > maxWeight)
        {
            throw new InvalidOperationException($"Pet is too heavy. Maximum allowed weight is {maxWeight}kg.");
        }

        if (petDetails.Weight > maxCabinWeight)
        {
            petDetails.SeatingLocation = "Luggage Room";
            Console.WriteLine($"Due to weight ({petDetails.Weight}kg), pet will be transported in luggage compartment.");
        }
        else if (petDetails.Type == "Dog" || petDetails.Type == "Cat")
        {
            Console.WriteLine("Would you like the pet to travel in cabin? (y/n):");
            petDetails.SeatingLocation = Console.ReadLine()?.ToLower().StartsWith("y") ?? false ? "Seat" : "Luggage Room";
        }
        else
        {
            petDetails.SeatingLocation = "Luggage Room";
        }
    }
}