using System.Collections.Generic;

public class PetService
{
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
        if (petDetails.Type == "Dog" || petDetails.Type == "Cat")
        {
            if (petDetails.SeatingLocation != "Seat")
            {
                throw new InvalidOperationException("Dogs and cats must sit in the cabin.");
            }
        }
        else
        {
            if (petDetails.SeatingLocation != "Luggage Room")
            {
                throw new InvalidOperationException("Other animals must go in the luggage room.");
            }
        }
    }
} 