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
        while (true)
        {
            if (!_petWeightLimits.ContainsKey(petDetails.Type))
            {
                Console.WriteLine("Invalid pet type. Please enter a valid pet type (Dog, Cat, Other):");
                petDetails.Type = Console.ReadLine();
                continue;
            }

            var (maxCabinWeight, maxWeight) = _petWeightLimits[petDetails.Type];

            if (petDetails.Weight > maxWeight)
            {
                Console.WriteLine($"Pet is too heavy. Maximum allowed weight is {maxWeight}kg. Please enter a valid weight:");
                if (double.TryParse(Console.ReadLine(), out var newWeight))
                {
                    petDetails.Weight = newWeight;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a numerical value.");
                }
                continue;
            }

            if (petDetails.Weight > maxCabinWeight)
            {
                petDetails.SeatingLocation = "Luggage Room";
                Console.WriteLine($"Due to weight ({petDetails.Weight}kg), the pet will be transported in the luggage compartment.");
            }
            else if (petDetails.Type == "Dog" || petDetails.Type == "Cat")
            {
                Console.WriteLine("Would you like the pet to travel in the cabin? (y/n):");
                string? response = Console.ReadLine()?.ToLower();
                petDetails.SeatingLocation = response == "y" ? "Seat" : "Luggage Room";
            }
            else
            {
                petDetails.SeatingLocation = "Luggage Room";
            }

            break;
        }
    }
}